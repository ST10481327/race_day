# RaceDay_poe_part3
# RaceDay Platform

## 📖 Project Overview
The **RaceDay Platform** is a comprehensive event management system designed for running, walking, and cycling events. The system is built using a multi-tier architecture:
- **Part 2: RESTful API** (ASP.NET Core Web API + SQL Server) handling all business logic and data access.
- **Part 3: MVC Front-End** (ASP.NET Core MVC) consuming the API with session-based authentication and Azure Blob Storage.

## 👥 The Two Roles
The system enforces strict role-based access control using ASP.NET Core Sessions:

1. **Organizer**
   - Create, update, and delete their own events.
   - Upload event banner images (stored in Azure Blob Storage).
   - Add categories (e.g., 10km, Senior) to events.
   - View all enrolments for their events.
   - Approve or reject participant enrolments.
   - Capture finish times and positions for participants.

2. **Participant**
   - View all available events.
   - Enrol in events by selecting a specific category.
   - Withdraw their own enrolment.
   - View their own race results (finish times and positions).

## 🛠️ Setup and Run Instructions

### Prerequisites
- Visual Studio 2022 (.NET 8.0 SDK)
- SQL Server Management Studio (SSMS)
- Azure Storage Account (for Blob Storage)

### 1. Database Setup
1. Open SSMS and connect to your local server.
2. Run the SQL script provided in the `/docs` folder to create the `race_day` database and seed the initial data.

### 2. Azure Blob Storage Setup
1. Create a Storage Account in Azure and create a container named `images`.
2. Copy the Connection String.
3. Open `appsettings.json` in the **API project** (`RaceDay_poe_part2`) and paste your connection string:
```json
"AzureStorage": {
    "ConnectionString": "YOUR_AZURE_CONNECTION_STRING",
    "ContainerName": "images"
}

<img width="1358" height="252" alt="image" src="https://github.com/user-attachments/assets/49419f0a-e3a6-4f5b-8261-81e11149c18e" />
