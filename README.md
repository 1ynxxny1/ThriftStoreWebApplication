# Thrift Store Web Application

This is a web application for online thrift shopping, built with ASP.NET Core MVC. The app allows users to browse, filter, and purchase second-hand items, while the admin can manage products, users, and orders.

## Features
- **Admin Module**:
  - Add, edit, delete products
  - Manage users (edit roles, delete users)
  - View and manage orders (payment status, order status)
- **Client Module**:
  - Register, log in, reset/change password
  - Browse products with filters (brand, gender, size, category, price)
  - Add items to the cart and checkout
  - View order history

## Prerequisites
- .NET 8 SDK
- Visual Studio 2022
- SQL Server 2019
- Git

## Installation

### 1. Clone the Repository
```bash
git clone https://github.com/1ynxxny1/ThriftStoreWebApplication.git
cd ThriftStoreWebApplication
```
### 2. Open the Project
- Open the solution `(.sln)` file in Visual Studio 2022.

### 3. Configure the Database
- Update the connection string in `appsettings.json` under the `"ConnectionStrings"` section:
```json
  "ConnectionStrings": {
  "DefaultConnection": "Data Source=YOUR_SQL_SERVER;Initial Catalog=ThriftStoreDB;Integrated Security=True;Trust Server Certificate=True"
}
```
- Run the following command in the Package Manager Console to apply migrations:
```bash
Update-Database
```

### 4. Configure Brevo (SendinBlue) API for Emails
- In `appsettings.json`, add Brevo API key:
```json
"BrevoSettings": {
  "ApiKey": "BREVO_API_KEY_PROVIDED_IN_EMAIL",
  "SenderName": "Thrift Avenue",
  "SenderEmail": "dea.bibovska@gmail.com"
},
```

### 6. Admin Access
- After registering, use the database to update your user’s role to "Admin".
- In the `AspNetUserRoles` table, manually assign the `Admin` role to your user.
