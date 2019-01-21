#pragma once
#include <string>
#include <chrono>
enum Status
{
	RED,
	ORANGE,
	GREEN,
	OFF
};

enum Type
{
	CAR,
	PEDESTRIAN,
	BIKE,
	BUS,
	TRAIN,
	UNDEFINED
};

class TrafficLight
{
public:
	TrafficLight(std::string name);
	~TrafficLight();
	std::string lightName;
	Status status;
	Type type;
	int timer;
	bool Changed = false;
	std::string GetColor(Status status);
	void StartTimer();
	int CheckTimer();
	std::chrono::steady_clock::time_point start;
	std::chrono::steady_clock::time_point end;
	bool blocked;
private:
	Type getType(std::string name);
};

TrafficLight::TrafficLight(std::string name) : status(RED), type(UNDEFINED), timer(0), blocked(false)
{
	lightName = name;
	type = getType(name);
}

TrafficLight::~TrafficLight()
{
}

Type TrafficLight::getType(std::string name)
{
	switch (name.at(0))
	{
	case 'A':
		return Type::CAR;
	case 'B':
		return Type::BIKE;
	case 'C':
		return Type::PEDESTRIAN;
	case 'D':
		return Type::BUS;
	case 'F':
		return Type::TRAIN;
	default:
		return Type::UNDEFINED;
	}
}

std::string TrafficLight::GetColor(Status status)
{
	switch (status)
	{
	case RED:
		return "red";
	case ORANGE:
		return "orange";
	case GREEN:
		return "green";
	case OFF:
		return "black";
	}
}

void TrafficLight::StartTimer()
{
	start = std::chrono::steady_clock::now();
}

int TrafficLight::CheckTimer()
{
	end = std::chrono::steady_clock::now();
	return std::chrono::duration_cast<std::chrono::seconds>(end - start).count();
}