#include "TrafficLight.h"
#include <iostream>
#include <string>
#include <vector>
#include <queue>
#include <list>
#include <thread>
#include <boost/property_tree/ptree.hpp>
#include <boost/property_tree/json_parser.hpp>
#include <boost/signals2.hpp>
#include "json.hpp"
//#include <nlohmann/json.hpp>
#include "TrafficLightStateMachine.h"

const int RedTime = 2;
const int GreenTime = 8;
const int OrangeTime = 4;
namespace pt = boost::property_tree;
std::vector<TrafficLight> lights;

bool TrafficLightStateMachine::CheckConflicts(int index)
{
	if (lights[index].type == CAR)
	{
		for (int i = 0; i < CarConflicts.size(); i++)
		{
			if (CarConflicts[i][0] == lights[index].lightName)
			{
				for (int x = 1; x < CarConflicts[i].size(); x++)
				{
					for (int y = 0; y < lights.size(); y++)
					{
						if (CarConflicts[i][x] == lights[y].lightName)
						{
							if (lights[y].status == GREEN || lights[y].status == ORANGE)
							{
								return false;
							}
							else
							{
								break;
							}
						}
					}
					
				}
				return true;
			}
		}
		
	}
	else if (lights[index].type == BIKE)
	{
		for (int i = 0; i < BikeConflicts.size(); i++)
		{
			if (BikeConflicts[i][0] == lights[index].lightName)
			{
				for (int x = 1; x < BikeConflicts[i].size(); x++)
				{
					for (int y = 0; y < lights.size(); y++)
					{
						if (BikeConflicts[i][x] == lights[y].lightName)
						{
							if (lights[y].status == GREEN || lights[y].status == ORANGE)
							{
								return false;
							}
						}
					}

				}
				return true;
			}
		}
	}
	else if (lights[index].type == PEDESTRIAN)
	{
		for (int i = 0; i < PedestrianConflicts.size(); i++)
		{
			if (PedestrianConflicts[i][0] == lights[index].lightName)
			{
				for (int x = 1; x < PedestrianConflicts[i].size(); x++)
				{
					for (int y = 0; y < lights.size(); y++)
					{
						if (PedestrianConflicts[i][x] == lights[y].lightName)
						{
							if (lights[y].status == GREEN || lights[y].status == ORANGE || lights[y].status == ORANGE)
							{
								return false;
							}
						}
					}

				}
				return true;
			}
		}
	}
	else if (lights[index].type == TRAIN)
	{
		for (int i = 0; i < TrainConflicts.size(); i++)
		{
			if (TrainConflicts[i][0] == lights[index].lightName)
			{
				for (int x = 1; x < TrainConflicts[i].size(); x++)
				{
					for (int y = 0; y < lights.size(); y++)
					{
						if (TrainConflicts[i][x] == lights[y].lightName)
						{
							if (lights[y].status == GREEN || lights[y].status == ORANGE)
							{
								return false;
							}
						}
					}

				}
				return true;
			}
		}		
	}
	else if (lights[index].type == BUS)
	{
		
			if (BusConflicts[0][0] == lights[index].lightName)
			{
				for (int x = 1; x < BusConflicts[0].size(); x++)
				{
					for (int y = 0; y < lights.size(); y++)
					{
						if (BusConflicts[0][x] == lights[y].lightName)
						{
							if (lights[y].status == GREEN || lights[y].status == ORANGE)
							{
								return false;
							}
						}
					}

				}
				return true;
			}
		
	}
}

bool TrafficLightStateMachine::Priority(std::string name)
{

	for (int i = 0; i < PriorityList.size(); i++)
	{
		if (PriorityList[i] == name)
		{
			return true;
		}
	}
	return false;
}

void TrafficLightStateMachine::OveridePriority(std::string name, bool blocking)
{
	for (int i = 0; i < PriorityList.size(); i++)
	{
		for (int x = 0; x < TrainConflicts.size(); x++)
		{
			if (TrainConflicts[x][0] == name)
			{
				for (int index = 1; index < TrainConflicts[x].size(); index++)
				{
					for (int y = 0; y < lights.size(); y++)
					{
						if (TrainConflicts[x][index] == lights[y].lightName && lights[y].status == GREEN)
						{
							lights[y].status = ORANGE;
							lights[y].StartTimer();
							lights[y].Changed = true;
							lights[y].blocked = blocking;
						}
						else if (TrainConflicts[x][index] == lights[y].lightName)
						{
							lights[y].blocked = blocking;
						}
					}
				}
				break;
			}
		}
		for (int x = 0; x < BusConflicts.size(); x++)
		{
			if (BusConflicts[x][0] == name)
			{
				for (int index = 1; index < BusConflicts[x].size(); index++)
				{
					for (int y = 0; y < lights.size(); y++)
					{
						if (BusConflicts[x][index] == lights[y].lightName && lights[y].status == GREEN)
						{
							lights[y].status = ORANGE;
							lights[y].StartTimer();
							lights[y].Changed = true;
							lights[y].blocked = blocking;
						}
						else if (BusConflicts[x][index] == lights[y].lightName)
						{
							lights[y].blocked = blocking;
						}
					}
				}
				break;
			}
		}
	}
}

void TrafficLightStateMachine::ProcessTraffic()
{
	std::list<std::string> temp;
	for (auto i = registrations.begin(); i != registrations.end();)
	{
		for (int x = 0; x < lights.size(); x++)
		{			
			if (lights[x].lightName == *i)
			{
				std::cout << *i << std::endl;
				if (Priority(lights[x].lightName))
				{
					OveridePriority(lights[x].lightName, true);
				}

				if (lights[x].status == RED && CheckConflicts(x) && lights[x].CheckTimer() >= RedTime && !lights[x].blocked)
				{
					lights[x].status = GREEN;
					lights[x].StartTimer();
					lights[x].Changed = true;
				}
				else if (lights[x].status == GREEN && lights[x].CheckTimer() >= GreenTime)
				{
					lights[x].status = ORANGE;
					lights[x].StartTimer();
					lights[x].Changed = true;
				}
				else if (lights[x].status == ORANGE && lights[x].CheckTimer() >= OrangeTime)
				{
					if (lights[x].lightName.at(0) == 'F' || lights[x].lightName.at(0) == 'D')
					{
						if (lights[x].lightName == "F2")
						{
							std::replace(registrations.begin(), registrations.end(), "F2", "X1");
						}
						else if (lights[x].lightName == "F1")
						{
							std::replace(registrations.begin(), registrations.end(), "F1", "X2");
						}
						else if (lights[x].lightName == "D1")
						{
							std::replace(registrations.begin(), registrations.end(), "D1", "X3");
						}
						OveridePriority(lights[x].lightName, false);
					}
					else
					{
						temp.push_back(*i);
					}
					lights[x].status = RED;
					lights[x].StartTimer();
					lights[x].Changed = true;
				}
				break;
			}

						
		}
		++i;
	}
	for (auto i = temp.begin(); i != temp.end();)
	{
		for (auto x = registrations.begin(); x != registrations.end();)
		{
			if (*i == *x)
			{
				registrations.erase(x);
				break;
			}
			++x;
		}
		++i;
	}
	GetMessage();
}


TrafficLightStateMachine::TrafficLightStateMachine() : registrations({ "X1", "X2", "X3" })
{
	CreateCarLights();
	CreateBikeLights();
	CreateBusLights();
	CreatePedestrianLights();
	CreateTrainLights();
	
	std::thread([&] {while (true) { std::this_thread::sleep_for(std::chrono::milliseconds(100)); ProcessTraffic(); }}).detach();
}


TrafficLightStateMachine::~TrafficLightStateMachine()
{

}

void TrafficLightStateMachine::CreateCarLights()
{
	std::string CarLights[10]{ "A1", "A2", "A3", "A4", "A5", "A6", "A7", "A8", "A9", "A10" };

	for (int i = 0; i < 10; i++)
	{
		lights.push_back(TrafficLight(CarLights[i]));
	}
}

void TrafficLightStateMachine::CreateBikeLights()
{
	std::string BikeLights[3]{ "B1", "B2", "B3" };

	for (int i = 0; i < 3; i++)
	{
		lights.push_back(TrafficLight(BikeLights[i]));
	}
}

void TrafficLightStateMachine::CreatePedestrianLights()
{
	std::string PedestrianLights[6]{ "C1.1", "C1.2", "C2.1", "C2.2", "C3.1", "C3.2" };

	for (int i = 0; i < 6; i++)
	{
		lights.push_back(TrafficLight(PedestrianLights[i]));
	}
}

void TrafficLightStateMachine::CreateBusLights()
{
	std::string BusLights[1]{ "D1" };

	for (int i = 0; i < 1; i++)
	{
		lights.push_back(TrafficLight(BusLights[i]));
	}
}

void TrafficLightStateMachine::CreateTrainLights()
{
	std::string TrainLights[2]{ "F1", "F2" };

	for (int i = 0; i < 2; i++)
	{
		lights.push_back(TrafficLight(TrainLights[i]));
	}
}


void TrafficLightStateMachine::CurrentTrafficState()
{
	if (SendString != LastString)
	{	
		LastString = SendString;
		Changed();
	}
}

void TrafficLightStateMachine::GetMessage()
{	
	bool first = false;
	nlohmann::json j1;
	nlohmann::json j2;
	for (int i = 0; i < lights.size(); i++)
	{	
		if (lights[i].Changed == true)
		{
			if (first)
			{
				j2 =
				{
							{"light", lights[i].lightName},
							{"status",	lights[i].GetColor(lights[i].status)},
							{"timer", lights[i].timer}
				};
				j1.insert(j1.end(), j2);
			}
			else
			{
				j1 = {
						{
							{"light", lights[i].lightName},
							{"status",	lights[i].GetColor(lights[i].status)},
							{"timer", lights[i].timer}
						}
				};
				first = true;				
			}
		}		
	}	
	SendString = j1.dump();

	CurrentTrafficState();
	
}
