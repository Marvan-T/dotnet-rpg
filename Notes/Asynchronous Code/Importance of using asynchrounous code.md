# void vs returning task (perils of not using async/await)


I ran into an error and here is the step by step of error:
1. **Initialization & Compilation**
    - EF is initializing the `DataContext` using the SQL Server provider.
    - EF is compiling a query to retrieve a `Book` where its `GoogleBooksId` matches a given parameter.

2. **Command & Connection Creation**
    - EF is preparing the SQL command to be executed against the database.
    - A database connection is being created. This connection is successfully created in 46ms.
    - The SQL command for reading the data is successfully created in 76ms and initialized in 144ms.

3. **Opening Connection**
    - EF tries to open a connection to the database `bestreads-recommendations` on the server `.\\SqlExpress`.

4. **Error & Cleanup**
    - Without further details, we see a generic error: "An error occurred using the connection to database 'bestreads-recommendations' on server '.\SqlExpress'."
    - The `DataContext` is then disposed of, and the connection to the database is closed.

5. **Query Cancellation**
    - Due to the error, the query is canceled.

The reason for this error is as follows:

- It got nothing to do with connection strings or database connections

```csharp
// Original signature on the controller:
[HttpPost]
public async void GetBookRecommendations(GetBookRecommendationsDto bookRecommendationsDto) {}
```

To fix the issue i had to change the signature to return a `Task`

```csharp
[HttpPost]
public async Task<ActionResult<ServiceResponse<IList<BookRecommendationDto>>>> GetBookRecommendations(GetBookRecommendationsDto bookRecommendationsDto) {}
```

## Explanation: 

Let's break down why changing the signature from `async void` to `async Task` (or `async Task<T>`) fixed the issue:

1. **Unhandled Exceptions in async void**:
    - Methods declared as `async void` are "fire-and-forget" by nature. The framework or caller cannot await their completion.
    - If an exception occurs inside an `async void` method, that exception cannot be caught outside of that method, making exception handling very difficult. This is different from `async Task` methods where exceptions are stored into the returned task, allowing callers to catch the exceptions.

2. **Request Life Cycle**:
    - In ASP.NET Core, when an action is invoked, it's executed within the context of an HTTP request. The framework knows to wait for an action to complete if it returns a `Task` or `Task<T>`. It then continues with the rest of the HTTP pipeline (like sending the response back to the client).
    - For an `async void` action, the framework doesn't know it needs to wait. Once the action is invoked, the framework thinks the action is complete and moves on. This can result in the request being closed prematurely before your asynchronous operation (like the database call) completes. If the request is closed and cleaned up before the operation completes, it can result in unexpected behaviors like the one you saw.

3. **Task Cancellation**:
    - Since the framework thinks the action has completed (due to `async void`), it may clean up resources related to that request, potentially causing the task within the action to be canceled, resulting in a `TaskCanceledException`.

In essence, `async void` was originally designed for event handlers and isn't suitable for server-side code where you have a clear start and end to the operation. Always use `async Task` or `async Task<T>` in ASP.NET Core actions to ensure the framework waits for the asynchronous operations to complete and to allow for proper exception handling.


