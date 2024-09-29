# LongRunningJob# LongRunningJob Solution

## Overview

This is a full-stack solution that simulates a long-running job using:
- **Backend**: ASP.NET Core Web API
- **Frontend**: Angular (latest version)

The application allows users to input text, process it on the server, and receive results progressively on the frontend. The backend simulates a lengthy task with random delays between sending each character of the processed string. Additionally, the frontend supports cancelling the task in real time using SignalR.

## Solution Structure

- **LongRunningJob**: The ASP.NET Core Web API project.
- **LongRunningJob.Client**: The Angular frontend application.

---

## Backend (LongRunningJob)

The backend is built using ASP.NET Core and exposes a SignalR hub for real-time communication with the frontend. It processes text input from the user and sends characters back to the frontend one by one with a random delay to simulate a long-running task.

### Features:
- Simulates long-running tasks.
- Sends processed characters progressively.
- Supports task cancellation.
- Built with ASP.NET Core SignalR for real-time communication.

### Technologies:
- **ASP.NET Core 8.0**
- **SignalR** for real-time communication
- **C#** as the primary language

### Endpoints:
- **/processing** (SignalR Hub): Handles text processing requests and sends characters one by one to the frontend.

### How to Run the Backend:
1. **Install .NET Core SDK 8.0** if you haven't already.
   
   [Download .NET SDK](https://dotnet.microsoft.com/download)

2. **Navigate to the backend project folder:**

   ```bash
   cd LongRunningJob
   dotnet restore
   dotnet run
   ```

3. **Backend will be available at http://localhost:5052.**

## Frontend (LongRunningJob.Client)

The frontend is an Angular application that connects to the ASP.NET Core backend using SignalR. It provides an interface where users can enter text, initiate the processing task, view the progress in real time, and cancel the task.

### Features:

- Real-time text processing via SignalR.
- Displays processed characters one by one.
- Cancel the processing task in real-time.
- Responsive UI with Angular Material for enhanced design.

### Technologies:

- Angular (latest version)
- TypeScript as the primary language
- Angular Material for UI components
- SignalR Client for real-time communication with the backend

### How to Run the Frontend:

1. **Install Node.js and npm if you haven’t already.**

2. **Navigate to the frontend project folder:**

```bash
cd LongRunningJob.Client
```

3. **Install frontend dependencies:**

```bash
npm install
```

4. **Run the Angular development server:**

```bash
ng serve
```

5. **Frontend will be available at http://localhost:4200.**

### Running the Whole Solution

1.	**Start the Backend:**
- Follow the instructions under the Backend section.
2.	**Start the Frontend:**
- Follow the instructions under the Frontend section.

Once both are running:

- Navigate to http://localhost:4200 to use the frontend.
- The frontend communicates with the backend via SignalR on http://localhost:5052.
