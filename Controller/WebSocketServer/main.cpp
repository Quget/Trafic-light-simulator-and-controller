#include <boost/asio.hpp>
#include <boost/beast/core.hpp>
#include <boost/beast/websocket.hpp>
#include <boost/asio/ip/tcp.hpp>
#include <cstdlib>
#include <functional>
#include <iostream>
#include <string>
#include <thread>
#include <sstream>
#include <boost/property_tree/ptree.hpp>
#include <boost/property_tree/json_parser.hpp>
#include <boost/foreach.hpp>
#include <boost/asio/buffers_iterator.hpp>
#include <boost/signals2.hpp>
#include "TrafficLightStateMachine.h"

// Short alias for this namespace
namespace pt = boost::property_tree;

// Create a root
// Load the json file in this ptree

using tcp = boost::asio::ip::tcp;               // from <boost/asio/ip/tcp.hpp>
namespace websocket = boost::beast::websocket;  // from <boost/beast/websocket.hpp>

void fail(boost::system::error_code ec, char const* what)
{
	std::cerr << what << ": " << ec.message() << "\n";
}

// Echoes back all received WebSocket messages
class session : public std::enable_shared_from_this<session>
{
public:
	websocket::stream<tcp::socket> ws_;
	boost::asio::strand<boost::asio::io_context::executor_type> strand_;
	boost::asio::strand<boost::asio::io_context::executor_type> strand1_;
	boost::beast::multi_buffer buffer_;
	boost::beast::multi_buffer writebuffer_;
	TrafficLightStateMachine stateMachine;
	int size = 0;
	pt::ptree root;
	bool started;

	// Take ownership of the socket
	explicit session(tcp::socket socket) : ws_(std::move(socket)), strand_(ws_.get_executor()), strand1_(ws_.get_executor()), started(false)
	{
		stateMachine.Changed.connect(std::bind(&session::do_write, this));
	}
	
	// Start the asynchronous operation
	void run()
	{
		// Accept the websocket handshake
		ws_.async_accept(
			boost::asio::bind_executor(
				strand_,
				std::bind(
					&session::on_accept,
					shared_from_this(),
					std::placeholders::_1)));		
	}

	void on_accept(boost::system::error_code ec)
	{
		if (ec)
			return fail(ec, "accept");
		// Read a message
		do_read();
	}

	void do_read()
	{
		// Read a message into our buffer
		ws_.async_read(
			buffer_,
			boost::asio::bind_executor(
				strand_,
				std::bind(
					&session::on_read,
					shared_from_this(),
					std::placeholders::_1,
					std::placeholders::_2)));
	}

	void on_read(boost::system::error_code ec,std::size_t bytes_transferred)
	{
		boost::ignore_unused(bytes_transferred);

		// This indicates that the session was closed
		if (ec == websocket::error::closed)
		{
			stateMachine.Changed.disconnect_all_slots();
			return;
		}

		if (ec)
		{
			fail(ec, "read");
			stateMachine.Changed.disconnect_all_slots();
			return;
		}

		// Echo the message
		ws_.text(ws_.got_text());

		try
		{
			std::stringstream ss;
			ss << boost::beast::buffers_to_string(buffer_.data());
			pt::read_json(ss, root);

			BOOST_FOREACH(pt::ptree::value_type &v, root.get_child("")) {
				RegisterTrafficLight(v.second.data());
			}
			buffer_.consume(buffer_.size());
			do_read();
		}
		catch (std::exception const &e)
		{
			std::cerr << "Error Parsing JSON: " << e.what() << std::endl;
		}
		
	}

	void do_write()
	{
		std::string const contents = stateMachine.SendString;

		size_t n = buffer_copy(writebuffer_.prepare(contents.size()), boost::asio::buffer(contents));
		writebuffer_.commit(n);

		ws_.async_write(
			writebuffer_.data(),
			boost::asio::bind_executor(
				strand1_,
				std::bind(
					&session::on_write,
					shared_from_this(),
					std::placeholders::_1,
					std::placeholders::_2)));
	}

	void RegisterTrafficLight(std::string light)
	{
		std::cerr << "Register" << std::endl;
		if (stateMachine.registrations.empty())
		{
			if (light.at(0) == 'F' || light.at(0) == 'D')
			{
				if (light == "F2")
				{
					std::replace(stateMachine.registrations.begin(), stateMachine.registrations.end(), "X1", "F2");
				}
				else if (light == "F1")
				{
					std::replace(stateMachine.registrations.begin(), stateMachine.registrations.end(), "X2", "F1");
				}
				else if (light == "D1")
				{
					std::replace(stateMachine.registrations.begin(), stateMachine.registrations.end(), "X3", "D1");
				}
			}
			else
			{
				stateMachine.registrations.push_back(light);
			}
			size++;
		}
		else
		{
			for (auto const& i : stateMachine.registrations)
			{
				std::string compare = i;
				if (compare == light)
				{
					return;
				}
			}
			if (light.at(0) == 'F' || light.at(0) == 'D')
			{
				if (light == "F2")
				{
					std::replace(stateMachine.registrations.begin(), stateMachine.registrations.end(), "X1", "F2");
				}
				else if (light == "F1")
				{
					std::replace(stateMachine.registrations.begin(), stateMachine.registrations.end(), "X2", "F1");
				}
				else if (light == "D1")
				{
					std::replace(stateMachine.registrations.begin(), stateMachine.registrations.end(), "X3", "D1");
				}
			}
			else
			{
				stateMachine.registrations.push_back(light);
			}
			size++;
		}
	}

	void on_write(boost::system::error_code ec, std::size_t bytes_transferred)
	{
		boost::ignore_unused(bytes_transferred);

		if (ec)
			return fail(ec, "write");

		writebuffer_.consume(writebuffer_.size());
	}
};

// Accepts incoming connections and launches the sessions
class listener : public std::enable_shared_from_this<listener>
{
	tcp::acceptor acceptor_;
	tcp::socket socket_;

public:
	listener(boost::asio::io_context& ioc,tcp::endpoint endpoint): acceptor_(ioc), socket_(ioc)
	{
		boost::system::error_code ec;

		// Open the acceptor
		acceptor_.open(endpoint.protocol(), ec);
		if (ec)
		{
			fail(ec, "open");
			return;
		}

		// Allow address reuse
		acceptor_.set_option(boost::asio::socket_base::reuse_address(true), ec);
		if (ec)
		{
			fail(ec, "set_option");
			return;
		}

		// Bind to the server address
		acceptor_.bind(endpoint, ec);
		if (ec)
		{
			fail(ec, "bind");
			return;
		}

		// Start listening for connections
		acceptor_.listen(
			boost::asio::socket_base::max_listen_connections, ec);
		if (ec)
		{
			fail(ec, "listen");
			return;
		}
	}

	// Start accepting incoming connections
	void run()
	{
		if (!acceptor_.is_open())
			return;
		do_accept();
	}

	void do_accept()
	{
		acceptor_.async_accept(
			socket_,
			std::bind(
				&listener::on_accept,
				shared_from_this(),
				std::placeholders::_1));
	}

	void on_accept(boost::system::error_code ec)
	{
		if (ec)
		{
			fail(ec, "accept");
		}
		else
		{
			// Create the session and run it
			std::make_shared<session>(std::move(socket_))->run();
			std::cout << "Connection established" << std::endl;
		}

		// Accept another connection
		do_accept();
	}
};

//------------------------------------------------------------------------------


int main(int argc, char* argv[])
{
	// Check command line arguments.
	boost::asio::ip::address address;
	boost::asio::io_service io_service;
	tcp::resolver resolver(io_service);
	tcp::resolver::query query(boost::asio::ip::host_name(), "");
	tcp::resolver::iterator iter = resolver.resolve(query);
	tcp::resolver::iterator end; // End marker.
	while (iter != end)
	{
		tcp::endpoint ep = *iter++;
		if (iter == end)
		{
			address = ep.address();

		}
	}
	auto const port = static_cast<unsigned short>(8080);
	auto const threads = std::max<int>(1, 8);
	std::cout << address << std::endl;
	// The io_context is required for all I/O
	boost::asio::io_context ioc{ threads };

	// Create and launch a listening port
	std::make_shared<listener>(ioc, tcp::endpoint{ address, port })->run();

	// Run the I/O service on the requested number of threads
	std::vector<std::thread> v;
	v.reserve(threads - 1);
	for (auto i = threads - 1; i > 0; --i)
		v.emplace_back(
			[&ioc]
	{
		ioc.run();
	});
	ioc.run();

	return EXIT_SUCCESS;
}