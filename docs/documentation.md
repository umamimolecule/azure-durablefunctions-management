# Documentation

### Table of Contents
 - [Start New Instance](#startnewinstance)  
 - [Query instances](#queryinstances)  
 - [Query all instances](#queryallinstances)  
 - [Terminate instances](#terminateinstances)  
 - [Send events to instances](#sendeventstoinstances)  
 - [Retrieve HTTP management webhook URLs](#retrievewebhookurls)  
 - [Rewind instances (preview)](#rewindinstances)  
 - [Purge instance history](#purgeinstancehistory)  

---
<a name="startnewinstance"/>

## Start new instance

Starts a new instance. Internally, this method enqueues a message into the control queue, which then triggers the start of a function with the specified name that uses the orchestration trigger binding.

`[POST] http://localhost:7071/api/orchestration/instances/startNew`

**Query parameters**
 - OrchestratorFunctionName (**Required**): The name of the orchestrator function to start.
 - InstanceId (**Optional**): The ID to use for the new orchestration instance.  If not supplied, a random unique value will be automatically chosen.
 
**Payload**

Payload is any JSON body you want to pass into the orchestration instance.

**Response**
```
200 OK

{
   "instanceId": "acb2c631bf484b9da179e5a03d96f149"
}
```
 - **instanceId**: The ID of the new orchestration instance.

**Example**

```
POST http://localhost:7071/api/orchestration/instances/startNew?orchestratorFunctionName=MyDurableFunction&instanceId=manualTrigger123

Content-Type: application/json

Body:
{
   "id": 1,
   "name": "Fred",
   "description": "This is the input which is sent to the durable function"
}
```

**Notes**

From [Microsoft's documentation](https://docs.microsoft.com/en-us/azure/azure-functions/durable/durable-functions-instance-management#start-instances) regarding specifying instance IDs:

>*Use a random identifier for the instance ID. Random instance IDs help ensure an equal load distribution when you're scaling orchestrator functions across multiple VMs. The proper time to use non-random instance IDs is when the ID must come from an external source, or when you're implementing the singleton orchestrator pattern.*


<a name="queryinstances" />

## Query instances

Returns information about the status of an orchestration instance.

`[GET] http://localhost:7071/api/orchestration/instances/{instanceId}`

**Query parameters**
 - showHistory (**Optional**): If set to true, the response contains the execution history.
 - showHistoryOutput (**Optional**): If set to true, the execution history contains activity outputs.
 - showInput (**Optional**): If set to false, the response won't contain the input of the function. The default value is true.

**Response**
```
200 OK

{
    "name": "YourFunction",
    "instanceId": "acb2c631bf484b9da179e5a03d96f149",
    "createdTime": "2019-11-13T09:30:10.000Z",
    "lastUpdatedTime": "2019-11-13T09:37:00.000Z",
    "input": {},
    "output": {},
    "runtimeStatus": "Running",
    "customStatus": {},
    "history": [
        {
            "EventType": "ExecutionStarted",
            "Timestamp": "2019-12-24T12:39:43.1086943Z",
            "FunctionName": "MyOrchestrator"
        },
        {
            "EventType": "TaskCompleted",
            "Timestamp": "2019-12-24T12:39:53.7277873Z",
            "ScheduledTime": "2019-12-24T12:39:43.5561944Z",
            "FunctionName": "PauseActivity"
        },
        {
            "EventType": "ExecutionCompleted",
            "OrchestrationStatus": "Completed",
            "Timestamp": "2019-12-24T12:39:53.9035172Z"
        }
    ]
}
```

 - **Name**: The name of the orchestrator function.
 - **InstanceId**: The instance ID of the orchestration (should be the same as the instanceId input).
 - **CreatedTime**: The time at which the orchestrator function started running.
 - **LastUpdatedTime**: The time at which the orchestration last checkpointed.
 - **Input**: The input of the function as a JSON value. This field isn't populated if showInput is false.
 - **CustomStatus**: Custom orchestration status in JSON format.
 - **Output**: The output of the function as a JSON value (if the function has completed). If the orchestrator function failed, this property includes the failure details. If the orchestrator function was terminated, this property includes the reason for the termination (if any).
 - **RuntimeStatus**: One of the following values:
   - **Pending**: The instance has been scheduled but has not yet started running.
   - **Running**: The instance has started running.
   - **Completed**: The instance has completed normally.
   - **ContinuedAsNew**: The instance has restarted itself with a new history. This state is a transient state.
   - **Failed**: The instance failed with an error.
   - **Terminated**: The instance was stopped abruptly.
 - **History**: The execution history of the orchestration. This field is only populated if showHistory is set to true.

**Example**

```
GET http://localhost:7071/api/orchestration/instances/acb2c631bf484b9da179e5a03d96f149?showHistory=true&showHistoryOutput=true&showInput=true
```

<a name="queryallinstances" />

## Query all instances

Query the statuses of all orchestration instances.

`[GET] http://localhost:7071/api/orchestration/instances`

**Query parameters**
 - RuntimeStatus (**Optional**): A comma-delimited list of status to filter on.  Example: `runtimeStatus=Running,Completed`
 - CreatedTimeFrom (**Optional**): A date/time string to filter from.  Example: `createdTimeFrom=2019-11-13T09:30:00.000Z`
 - CreatedTimeTo (**Optional**): A date/time string to filter until.  Example: `createdTimeFrom=2020-11-13T09:30:00.000Z`
 - TaskHubNames (**Optional**): A comma-delimited list of task hub names to filter on.
 - PageSize (**Optional**): The maximum number of results to return per request.
 - ContinuationToken (**Optional**): Retrieve a specific page of results.  Leave empty to retrieve the first page.
 - InstanceIdPrefix (**Optional**): Filters the instances whose prefix matches this value.
 - ShowInput (**Optional**): If set to true, will include the orchestration inputs for each instance.

**Response**
```
200 OK

{
    "durableOrchestrationState": [
      {
         "name": "YourFunction",
         "instanceId": "acb2c631bf484b9da179e5a03d96f149",
         "createdTime": "2019-11-13T09:30:10.000Z",
         "lastUpdatedTime": "2019-11-13T09:37:00.000Z",
         "input": {},
         "output": {},
         "runtimeStatus": "Running",
         "customStatus": {},
         "history": [
             {
                 "EventType": "ExecutionStarted",
                 "Timestamp": "2019-12-24T12:39:43.1086943Z",
                 "FunctionName": "MyOrchestrator"
             },
             {
                 "EventType": "TaskCompleted",
                 "Timestamp": "2019-12-24T12:39:53.7277873Z",
                 "ScheduledTime": "2019-12-24T12:39:43.5561944Z",
                 "FunctionName": "PauseActivity"
             },
             {
                 "EventType": "ExecutionCompleted",
                 "OrchestrationStatus": "Completed",
                 "Timestamp": "2019-12-24T12:39:53.9035172Z"
             }
         ]
      }
    ],
    "continuationToken": "bnVsbA=="
}
```
Each item in the durableOrchestrationState array follows the structure for [Query instances](#queryinstances) above.

<a name="terminateinstances" />

## Terminate instances

Terminates a running instance.

`[POST] http://localhost:7071/api/orchestration/instances/{instanceId}/terminate`

**Query parameters**
 - Reason (**Required**): A description of why the instance was terminated.

**Example**
```
POST http://localhost:7071/api/orchestration/instances/acb2c631bf484b9da179e5a03d96f149/terminate?reason=Instance%20taking%20too%20long
```

**Notes**

From [Microsoft's documentation](https://docs.microsoft.com/en-us/azure/azure-functions/durable/durable-functions-instance-management#terminate-instances) regarding when an instance is terminated:

>*A terminated instance stops running as soon as it reaches the next await (.NET) or yield (JavaScript) point, or it terminates immediately if it's already on an await or yield.*

<a name="sendeventstoinstances" />

## Send events to instances

Sends an event notification to a running instance.

`[POST] http://localhost:7071/api/orchestration/instances/{instanceId}/raiseEvent`

**Query parameters**
 - EventName (**Required**): The name of the event.
 - TaskHubName (**Optional**): The TaskHubName of the orchestration that will handle the event.
 - ConnectionName (**Optional**): The name of the connection string associated with taskHubName.

**Body**
Contains the payload for the event being sent.

**Response**

```
202 Accepted

No body content
```

**Example**
```
POST http://localhost:7071/api/orchestration/instances/acb2c631bf484b9da179e5a03d96f149/raiseEvent?eventName=MyEvent

Content-Type: application/json

Body:
{
   "id": 1,
   "name": "Fred",
   "description": "This is the payload that is sent to the event"
}
```

<a name="retrievewebhookurls" />

## Retrieve HTTP management webhook URLs

Retrieve HTTP management webhook URLs that external systems can communicate with Durable Functions through.

`[GET] http://localhost:7071/api/orchestration/instances/{instanceId}/managementWebhookUrls`

**Response**
```
{
    "id": "theInstanceId",
    "statusQueryGetUri": "http://hostname/runtime/webhooks/durabletask/instances/acb2c631bf484b9da179e5a03d96f149?taskHub=TestHubName&connection=Storage&code=base64encodedsecret",
    "sendEventPostUri": "http://hostname/runtime/webhooks/durabletask/instances/acb2c631bf484b9da179e5a03d96f149/raiseEvent/{eventName}?taskHub=TestHubName&connection=Storage&code=base64encodedsecret",
    "terminatePostUri": "http://hostname/runtime/webhooks/durabletask/instances/acb2c631bf484b9da179e5a03d96f149/terminate?reason={text}&taskHub=TestHubName&connection=Storage&code=base64encodedsecret",
    "rewindPostUri": "http://hostname/runtime/webhooks/durabletask/instances/acb2c631bf484b9da179e5a03d96f149/rewind?reason={text}&taskHub=TestHubName&connection=Storage&code=base64encodedsecret",
    "purgeHistoryDeleteUri": "http://hostname/runtime/webhooks/durabletask/instances/acb2c631bf484b9da179e5a03d96f149?taskHub=TestHubName&connection=Storage&code=base64encodedsecret"
}
```

 - **Id**: The instance ID of the orchestration (should be the same as the InstanceId input).
 - **StatusQueryGetUri**: The status URL of the orchestration instance.
 - **SendEventPostUri**: The "raise event" URL of the orchestration instance.
 - **TerminatePostUri**: The "terminate" URL of the orchestration instance.
 - **PurgeHistoryDeleteUri**: The "purge history" URL of the orchestration instance.


<a name="rewindinstances" />

## Rewind instance (Preview)

Rewinds a failed instance to a previously healthy state, by putting the orchestration back into the Running state. This method will also rerun the activity or sub-orchestration execution failures that caused the orchestration failure.

`[POST] http://localhost:7071/api/orchestration/instances/{instanceId}/rewind`

**Query parameters**
 - Reason (**Required**): A description of the reason for rewinding the instance.

**Response**
```
202 Accepted

No body content
```

**Example**
```
POST http://localhost:7071/api/orchestration/instances/acb2c631bf484b9da179e5a03d96f149/rewind?reason=Rollback%20due%20to%20failure
```

**Notes**

From [Microsoft's documentation](https://docs.microsoft.com/en-us/azure/azure-functions/durable/durable-functions-instance-management#rewind-instances-preview) regarding rewinding:

>*The rewind feature doesn't support rewinding orchestration instances that use durable timers.*

<a name="purgeinstancehistory" />

## Purge instance history

Removes all the data associated with an orchestration.  There are two endpoints that can achive this - one to remove for a specific instance, and one to remove for multiple instances based on a set of filter conditions.

### 1. Purge history for specific instance

Removes all the data associated with a specific orchestration instance.

`[POST] http://localhost:7071/api/orchestration/instances/{instanceId}/purgeInstanceHistory`

**Response**
```
200 OK

{
   "instancesDeleted": 1
}
```
 - **instancedDeleted**: The number of instances deleted.  Will be 0 if the specified instance was not found, or 1 if it was found and deleted.

**Example**
```
POST http://localhost:7071/api/orchestration/instances/acb2c631bf484b9da179e5a03d96f149/purgeInstanceHistory
```

**Notes**

From [Microsoft's documentation](https://docs.microsoft.com/en-us/azure/azure-functions/durable/durable-functions-instance-management#purge-instance-history) regarding purging:

>*For the purge history operation to succeed, the runtime status of the target instance must be Completed, Terminated, or Failed.*

### 2. Purge history for multiple instances using a filter

Removes all the data associated with orchestration instances that match specified criteria.

`[POST] http://localhost:7071/api/orchestration/instances/purgeInstanceHistory`

**Query parameters**
 - CreatedTimeFrom (**Required**): A date/time string to filter from.  Example: `createdTimeFrom=2019-11-13T09:30:00.000Z`
 - CreatedTimeTo (**Optional**): A date/time string to filter to.  Example: `createdTimeTo=2020-11-13T09:30:00.000Z`
 - RuntimeStatus (**Optional**): A comma-delimited list of status to filter on.  Valid values are: Completed, Terminated, or Failed.  Example: `runtimeStatus=Completed,Terminated,Failed`

 **Response**
```
200 OK

{
   "instancesDeleted": 3
}
```
 - **instancedDeleted**: The number of instances that matched the conditions and were deleted.

**Example**
```
POST http://localhost:7071/api/orchestration/instances/purgeInstanceHistory?createdTimeFrom=2019-11-13T09:30:00.000Z&createdTimeTo=2020-11-13T09:30:00.000Z&runtimeStatus=Completed,Terminated,Failed
```
**Notes**

From [Microsoft's documentation](https://docs.microsoft.com/en-us/azure/azure-functions/durable/durable-functions-instance-management#purge-instance-history) regarding purging:

>*For the purge history operation to succeed, the runtime status of the target instance must be Completed, Terminated, or Failed.*
