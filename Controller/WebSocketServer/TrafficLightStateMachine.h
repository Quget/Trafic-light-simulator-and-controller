#pragma once

class TrafficLightStateMachine
{
public:
	void CreateCarLights();
	void CreateBikeLights();
	void CreatePedestrianLights();
	void CreateBusLights();
	void CreateTrainLights();	

	std::vector<std::vector<std::string>> CarConflicts = {
			{"A1","C1.1", "B1", "C3.2", "B3", "D1", "A7", "A5"},
			{"A2","C1.1", "B1", "A10", "A9", "A8", "F1", "F2", "A5", "A7", "D1"},
			{"A3","C1.1", "B1", "B2", "C2.2", "A6", "A5", "A7", "A10", "A9", "D1"},
			{"A4","C2.1", "B2", "D1", "C1.2", "B1", "A6", "A10"},
			{"A5","C2.1", "B2", "D1", "C3.2", "B3", "A1", "A2", "A3", "A6", "A7", "A10"},
			{"A6","F1", "F2", "C2.2", "B2", "C1.2", "B1","A3", "A4", "A5", "D1", "A10", "A9"},
			{"A7","F1", "F2", "C3.2", "B3", "A1", "A2", "A3", "A5", "A9", "A10", "D1"},
			{"A8","C3.1", "B3", "F1", "F2", "A2"},
			{"A9","C3.1", "B3", "C2.2", "B2", "A2", "A3", "A6", "A7"},
			{"A10","C3.1", "B3", "C1.2", "B1", "A2", "A3", "A4", "A5", "A6", "A7", "D1"}
	};
	std::vector<std::vector<std::string>> BikeConflicts = {
		{"B1", "A1", "A2", "A3", "A4","A6", "A10"},
		{"B2", "D1", "A4", "A5", "A3", "A6", "A9"},
		{"B3", "A8", "A9", "A10", "A1", "D1", "A5", "A7"}
	};
	std::vector<std::vector<std::string>> PedestrianConflicts = {
		{"C1.1","A1", "A2", "A3"},
		{"C1.2","A4", "A6", "A10"},
		{"C2.1","A4", "A5", "D1"},
		{"C2.2","A3", "A6", "A9"},
		{"C3.1","A8", "A9", "A10"},
		{"C3.2","A1", "A5", "A7", "D1"}
	};
	std::vector<std::vector<std::string>> BusConflicts = {
		{"D1", "B2", "C2.1", "A1", "A2", "A3", "A4", "A5", "A6", "A7", "A10", "C3.2", "B3"}
	};
	std::vector<std::vector<std::string>> TrainConflicts = {
		{"F1", "A6", "A7", "A2", "A8"},
		{"F2", "A6", "A7", "A2", "A8", "F1"}
	};
	std::vector<std::string> PriorityList = {
		"D1", "F1", "F2"
	};
	boost::signals2::signal<void()> Changed;
	std::string SendString;
	std::list<std::string> registrations;
	
	//std::list<TrafficLight> changes;
	
	void CurrentTrafficState();
	void GetMessage();

	//bool CheckConflicts(std::string name);

	void ProcessTraffic();

	bool CheckConflicts(int index);

	bool Priority(std::string name);
	void OveridePriority(std::string name, bool blocking);

	//void ProcessTraffic();

	TrafficLightStateMachine();
	~TrafficLightStateMachine();

private:
	std::string LastString;

};

