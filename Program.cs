using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;



namespace InterviewTest {
    class DirectedGraph {
        private Dictionary<string, Node> nodes;

        public DirectedGraph(){
            nodes = new Dictionary<string, Node>();
        }

        public bool AddTown(string _town, string _route, int _cost) {
            bool insertionSuccessful = false;

            // town has been inserted at least once before
            if(nodes.ContainsKey(_town)) {
                
                // README Case 4: town + route combination already used
                if( nodes[_town].DoesRouteExist(_route) ) {
                    insertionSuccessful = false;
                }
                // README Case 3a: town and connecting town exist
                // README Case 3b: town exists, connecting town does not
                else{
                    bool townInsertTwo;
                    if ( nodes.ContainsKey(_route) ){
                        townInsertTwo = true;
                    }
                    else {
                        townInsertTwo = AddToDictionary(_route);
                    }
                    bool routeInsert = nodes[_town].AddRoute(_route, _cost);

                    insertionSuccessful = townInsertTwo && routeInsert;
                }
            }
            // completely new town
            else {
                // README Case 2: dictionary has at least one entry, but town hasn't been used before
                if (nodes.Count > 0) {
                    bool townInsertOne = AddToDictionary(_town);
                    bool townInsertTwo;
                    if ( nodes.ContainsKey(_route) ){
                        townInsertTwo = true;
                    }
                    else {
                        townInsertTwo = AddToDictionary(_route);
                    }
                    bool routeInsert = nodes[_town].AddRoute(_route, _cost);

                    insertionSuccessful = townInsertOne && townInsertTwo && routeInsert;
                }
                // README Case 1: empty dictionary 
                else {
                    bool townInsertOne = AddToDictionary(_town);
                    bool townInsertTwo = AddToDictionary(_route);
                    bool routeInsert = nodes[_town].AddRoute(_route, _cost);

                    insertionSuccessful = townInsertOne && townInsertTwo && routeInsert;
                }
            }

            return insertionSuccessful;
        }

        private bool AddToDictionary(string _town) {
            try {
                nodes.Add(_town, new Node(_town) );

                return true;
            }
            catch (ArgumentException) {
                Console.WriteLine("A node with key {0} already exists.", _town);

                return false;
            }
            catch {
                Console.WriteLine("Generic exception");

                return false;
            }
        }

        // Used to answer Outputs 1-5 from the sample in the prompt
        public int GetRouteCost(string _fromPosition, string _toPosition){
            try {
                int cost = nodes[_fromPosition].Routes()[_toPosition];

                return cost;
            }
            catch (KeyNotFoundException) {
                return -1;
            }
        }

        // Recursive part: Used to answer Output 6 from the sample in the prompt
        private void CalculateTrip(string _targetPosition, string _currentPosition, int _currentStopCount, int _maxStops, ref int _routeCount) {
            // Recursive Base Case
            if (_currentStopCount >= _maxStops) {
                if (_currentPosition == _targetPosition) {
                    _routeCount++;
                    return;
                }
                else {
                    return;
                }
            }

            // Check if current position counts as a route. If so, return
            if (_currentPosition == _targetPosition){
                _routeCount++;
                return;
            }

            foreach (KeyValuePair<string, int> r in nodes[_currentPosition].Routes()) {
                CalculateTrip(_targetPosition, r.Key, _currentStopCount + 1, _maxStops, ref _routeCount);
            }
        }

        // Used to answer Output 6 from the sample in the prompt
        public int GetTripsMatchingMaxItinerary(string _fromPosition, string _toPosition, int _maxStops) {
            int routeCount = 0;
            foreach (KeyValuePair<string, int> r in nodes[_fromPosition].Routes()) {
                CalculateTrip(_toPosition, r.Key, 1, _maxStops, ref routeCount);
            }

            return routeCount;
        }

        // Recursive part: Used to answer Output 7 from the sample in the prompt
        private void CalculateTripWithLength(string _targetPosition, string _currentPosition, string _currentItinerary, int _currentStopCount, int _exactStops, ref int _routeCount) {
            // Recursive Base Case
            if (_currentStopCount > _exactStops) {
                return;
            }

            // Check if current position counts as a route. If so, return
            if (_currentPosition == _targetPosition && _currentItinerary.Length == _exactStops + 1){
                _routeCount++;
                return;
            }

            // recurse for all available routes
            foreach (KeyValuePair<string, int> r in nodes[_currentPosition].Routes()) {
                CalculateTripWithLength(_targetPosition, r.Key, _currentItinerary + r.Key, _currentStopCount + 1, _exactStops, ref _routeCount);
            }
        }

        // Used to answer Output 7 from the sample in the prompt
        public int GetTripsMatchingExactItinerary(string _fromPosition, string _toPosition, int _exactStops) {
            int routeCount = 0;
            foreach (KeyValuePair<string, int> r in nodes[_fromPosition].Routes()) {
                CalculateTripWithLength(_toPosition, r.Key, _fromPosition + r.Key, 0, _exactStops, ref routeCount);
            }

            return routeCount;
        }

        public void GetAllPossibleRoutes(string _currentPosition, string _targetPosition, string _currentItinerary, int _currentStopCount, int _maxStops, ref List<string> _allRoutes){
            // recursive base case
            if (_currentStopCount > _maxStops){
                if (_currentPosition == _targetPosition) {
                    _allRoutes.Add(_currentItinerary);
                }

                return;
            }

            if (_currentPosition == _targetPosition) {
                _allRoutes.Add(_currentItinerary);
            }

            foreach (KeyValuePair<string, int> r in nodes[_currentPosition].Routes()) {
                GetAllPossibleRoutes(r.Key, _targetPosition, _currentItinerary + r.Key, _currentStopCount + 1, _maxStops, ref _allRoutes);
            }
        }

        public int GetShortestRouteLength(string _fromPosition, string _toPosition) {
            // NOTE(JR): It's safe to assume that the shortest path wouldn't be longer than the number of stops in the system minus 1
            int maxRecurse = nodes.Count - 1;
            List<string> allRoutes = new List<string>();

            foreach (KeyValuePair<string, int> r in nodes[_fromPosition].Routes()) {
                GetAllPossibleRoutes(r.Key, _toPosition, _fromPosition + r.Key, 1, maxRecurse, ref allRoutes);
            }

            int minRouteCost = Int32.MaxValue;
            foreach (string route in allRoutes){
                int currentRouteCost = 0;
                for(int i = 0; i < route.Length - 1; i++){
                    int segmentCost = GetRouteCost(route[i].ToString(), route[i + 1].ToString() );
                    if (segmentCost > 0){
                        currentRouteCost += segmentCost;
                    }
                }

                if (currentRouteCost < minRouteCost) {
                    minRouteCost = currentRouteCost;
                }
            }

            return minRouteCost;
        }

        public void GetAllPossibleRoutesLessThanCost(string _currentPosition, string _targetPosition, string _currentItinerary, int _currentCost, int _maxCost, ref List<string> _allRoutes){
            // recursive base case
            if (_currentCost > _maxCost){
                return;
            }

            if (_currentPosition == _targetPosition) {
                _allRoutes.Add(_currentItinerary);
            }

            foreach (KeyValuePair<string, int> r in nodes[_currentPosition].Routes()) {
                int segmentCost = GetRouteCost(_currentPosition, r.Key );
                if (segmentCost > 0){
                    GetAllPossibleRoutesLessThanCost(r.Key, _targetPosition, _currentItinerary + r.Key, _currentCost + segmentCost, _maxCost, ref _allRoutes);
                }
            }
        }

        public int GetAllRoutesLessThanCost(string _fromPosition, string _toPosition, int _maxCost) {
            List<string> allRoutes = new List<string>();

            foreach (KeyValuePair<string, int> r in nodes[_fromPosition].Routes()) {
                GetAllPossibleRoutesLessThanCost(r.Key, _toPosition, _fromPosition + r.Key, GetRouteCost(_fromPosition, r.Key), _maxCost, ref allRoutes);
            }

            return allRoutes.Count;
        }

        public void PrintGraphState() {
            foreach (KeyValuePair<string, Node> n in nodes) {
                Console.WriteLine("Town: {0}", n.Key);
                // do something with entry.Value or entry.Key
                foreach (KeyValuePair<string, int> r in nodes[n.Key].Routes()) {
                    Console.WriteLine("Route: {0}, Cost: {1}", r.Key, r.Value);
                }

                Console.WriteLine("\n -------------------- \n");
            }
        }

        private class Node {
            private string town;
            private Dictionary<string, int> routes;

            public Node(string _town) {
                routes = new Dictionary<string, int>();
                town = _town;
            }

            public bool DoesRouteExist(string _route) {
                return routes.ContainsKey(_route);
            }

            public Dictionary<string, int> Routes() {
                return routes;
            }

            public bool AddRoute(string _route, int _cost) {
                try {
                    routes.Add(_route, _cost);

                    return true;
                }
                catch (ArgumentException) {
                    Console.WriteLine("A route with key {0} already exists.", _route);

                    return false;
                }
                catch {
                    Console.WriteLine("Generic exception");

                    return false;
                }
            }
        }
    }

    class Program {
        static bool ValidateInput(string _line) {
            string pattern = @"[A-Z][A-Z][0-9]+\b";
            Match m = Regex.Match(_line, pattern, RegexOptions.IgnoreCase);
            
            return m.Success;
        }

        static bool ValidateOutput(string _line) {
            string[] splitLine = _line.Split(" ");
            string command = splitLine[0];

            if (command == "distance") {
                bool validLine = true;
                string pattern = @"[A-Z]\b";
                for (int i = 1; i < splitLine.Length; i++){
                    Match m = Regex.Match(splitLine[i], pattern, RegexOptions.IgnoreCase);
                    if (m.Success == false){
                        validLine = false;
                        break;
                    }
                }

                return validLine;
            }
            else if (command == "numtripsmax" || command == "numtripsexact" || command == "numdiff") {
                if (splitLine.Length != 4){
                    return false;
                }

                string numTripsPattern = @"[0-9]+\b";
                string townPattern = @"[A-Z]\b";
                Match m1 = Regex.Match(splitLine[1], numTripsPattern, RegexOptions.IgnoreCase);
                Match m2 = Regex.Match(splitLine[2], townPattern, RegexOptions.IgnoreCase);
                Match m3 = Regex.Match(splitLine[3], townPattern, RegexOptions.IgnoreCase);

                return m1.Success && m2.Success && m3.Success;
            }
            else if (command == "shortest") {
                if (splitLine.Length != 3){
                    return false;
                }

                string townPattern = @"[A-Z]\b";
                Match m1 = Regex.Match(splitLine[1], townPattern, RegexOptions.IgnoreCase);
                Match m2 = Regex.Match(splitLine[2], townPattern, RegexOptions.IgnoreCase);

                return m1.Success && m2.Success;
            }
            else {
                Console.WriteLine("Error: malformed command statement in text file: {0}", command);

                return false;
            }
        }

        static void ProcessInput(string _line, DirectedGraph _graph, StringBuilder _sb) {
            string town = _line[0].ToString();
            string route = _line[1].ToString();
            int distance;

            // Try to parse distance. If not int, exception will throw, add to malformed input and return
            // NOTE(JR): we *could* rely on the regex check, but if this were a real codebase, that would
            // be risky as the codebase grew and the entry points to this function more diverse
            try{
                distance = int.Parse( _line.Substring(2) );
            } 
            catch {
                _sb.Append(_line + "\n");
                
                return;
            }

            // Add split line as a key-value pair to hash table
            try {
                _graph.AddTown(town, route, distance);
            }
            catch {
                _sb.Append(_line + "\n");
            }
        }

        static void ProcessOutput(string _line, DirectedGraph _graph, int _outputCount) {
            string[] splitLine = _line.Split(" ");
            string command = splitLine[0];

            if (command == "distance") {
                bool routeFound = true;
                int totalDistance = 0;
                for (int i = 1; i < splitLine.Length - 1; i++) {
                    string fromPosition = splitLine[i];
                    string toPosition = splitLine[i + 1];
                    int cost = _graph.GetRouteCost(fromPosition, toPosition);

                    if (cost == -1) {
                        routeFound = false;
                        break;
                    }
                    else {
                        totalDistance += cost;
                    }
                    
                }

                if (routeFound) {
                    Console.WriteLine("Output #{0}: {1}", _outputCount, totalDistance);
                }
                else {
                    Console.WriteLine("Output #{0}: NO SUCH ROUTE", _outputCount);
                }
            }
            else if (command == "numtripsmax") {
                int maxStops = int.Parse( splitLine[1] );
                string fromPosition = splitLine[2];
                string toPosition = splitLine[3];
                int tripsFound = _graph.GetTripsMatchingMaxItinerary(fromPosition, toPosition, maxStops);

                Console.WriteLine("Output #{0}: {1}", _outputCount, tripsFound);
            }
            else if (command == "numtripsexact") {
                int exactStops = int.Parse( splitLine[1] );
                string fromPosition = splitLine[2];
                string toPosition = splitLine[3];
                int tripsFound = _graph.GetTripsMatchingExactItinerary(fromPosition, toPosition, exactStops);

                Console.WriteLine("Output #{0}: {1}", _outputCount, tripsFound);
            }
            else if (command == "shortest") {
                string fromPosition = splitLine[1];
                string toPosition = splitLine[2];
                int shortestRouteLength = _graph.GetShortestRouteLength(fromPosition, toPosition);

                Console.WriteLine("Output #{0}: {1}", _outputCount, shortestRouteLength);
            }
            else if (command == "numdiff") {
                int maxCost = int.Parse( splitLine[1] ) - 1;
                string fromPosition = splitLine[2];
                string toPosition = splitLine[3];

                int routeCount = _graph.GetAllRoutesLessThanCost(fromPosition, toPosition, maxCost);

                Console.WriteLine("Output #{0}: {1}", _outputCount, routeCount);
            }
            else {
                Console.WriteLine("Error: malformed line got through validation code: {0}", _line);
            }
        }

        static void Main(string[] args) {
            StringBuilder malformedInput = new StringBuilder("");
            StringBuilder malformedOutput = new StringBuilder("");
            
            // Build directed graph
            DirectedGraph graph = new DirectedGraph();
            StreamReader inputFile = new StreamReader( Path.Combine(Environment.CurrentDirectory, "input.txt") );
            string inputLine;
            while( ( inputLine = inputFile.ReadLine() ) != null ){  
                if( ValidateInput(inputLine) == true ){
                    ProcessInput(inputLine, graph, malformedInput); 
                }
                else {
                    malformedInput.Append(inputLine + "\n");
                }
            }
            inputFile.Close();

            StreamReader outputFile = new StreamReader( Path.Combine(Environment.CurrentDirectory, "output.txt") );
            string outputLine;
            int outputCount = 1;
            while( ( outputLine = outputFile.ReadLine() ) != null ){  
                if( ValidateOutput(outputLine) == true ){
                    ProcessOutput(outputLine, graph, outputCount);
                    outputCount++;
                }
                else {
                    malformedOutput.Append(outputLine + "\n");
                }
            }
            outputFile.Close(); 

            
            // Write out malformed input and output to an errors file. Useful for checking if regex is busting or genuine error in input file
            // NOTE(JR): Not called for by prompt, but useful for checking your data files for copy paste error, etc.
            // NOTE(JR): StreamWrite implements IDisposable interace. In conjunction with `using`, file.close() handled for you.
            using (StreamWriter errorFile = new StreamWriter(Path.Combine(Environment.CurrentDirectory, "input-errors.txt"))){
                errorFile.WriteLine(malformedInput.ToString());
            }
            using (StreamWriter errorFile = new StreamWriter(Path.Combine(Environment.CurrentDirectory, "output-errors.txt"))){
                errorFile.WriteLine(malformedInput.ToString());
            }
        }
    }
}
