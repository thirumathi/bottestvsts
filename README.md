
# BOT Testing with VSTS
Framework for bot testing with VSTS
This framework covers web and load testing approaches
- BOT Testing Basics 
- BOT Testing Strategy
- Knowing VSTS Web tests
- Load Testing feasibilities
- Load Testing Recommendations
- Framework components
- Action Items

#### BOT Testing basics
- Custom Extraction Rules
    - To extract the following info
	    - Conversation ID
	    - Message ID
	    - BOT ID

#### Test strategy
- Involves following components
	- VSTS Web Test
	- BOT Connector Service
	- BOT
	- MS APP Auth Services

#### Input Data
- CSV Files with Messages

#### Output
- Azure Table Storage Data with request info

### Knowing VSTS Web tests
- Declarative and consists of a series of HTTP requests
- Work at the protocol layer by issuing HTTP requests
- Do not run JavaScript
- Can simulate actions at runtime by using plug-ins, extraction rules, or coded Web tests
- Used to test the functionality of Web applications
- Can also be used to test BOTs as they are web based
- Used both in performance tests and stress tests
- Create by recording activities in a browser or by using the Web Test Editor manually
- Create functional tests and data-driven tests
- Create and run tests that can test the performance of your applications
- Use .NET languages for test authoring, debugging, and test extensibility

### Load Testing feasibilities
- Can perform load test on top of web test
- Supports data and configuration driven testing
- Supports custom extraction and validation
- Custom metrics extraction supportability

### Load testing Recommendations
- Not To use OOTB Connectors like Direct Line, SKYPE, Web Chat etc.
- Leverage Custom Connectors for load testing
- Parameterize as much as possible
- BOT works on async transactions, calculate response times based on message timestamps
- DO NOT rely on PAGE request/response times
- Log data in shared common location

### Test framework components
- Test Project
	- Includes Web test and performance test configurations
- Helper Library
	- Includes the extraction and validation rules
- BOT Connector
	- Custom connector service to support BOT Testing
	- To be deployed in Azure
- BOT Connector Test App
	- Tests the connector service
- Sample BOT
	- Replaced by the Actual bot to be tested

### Go Dos
- Identify test scenarios to be load tested
- Create relevant data models
- Identify the load test profile to be tested
	- Test mix options
	- Number of users
	- Load patterns
	- Test Duration
	- Think time
	- Warmup time
	- No. of controllers and agents
