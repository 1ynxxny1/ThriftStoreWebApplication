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

### 5. Enter data into the Products Table
- To add sample products to the database, you can run the provided backup database file or execute the following SQL script:
```sql
INSERT INTO [ThriftStoreDB].[dbo].[Products] 
([Name], [Brand], [Gender], [Size], [Category], [Price], [Description], [ImageFileName], [CreatedDate])
VALUES 
('Hello Kitty t-shirt', 'Missteen', 'Womens', 'Medium', 'Clothing', 350, 'Vintage Hello Kitty black Missteen t-shirt in excellent condition, made from a cotton blend.', 'WShirt.jpg', GETDATE()),
('Blue Jeans', 'Carhartt', 'Womens', '38', 'Clothing', 700, 'Vintage Carhartt blue carpenter jeans in very good condition. Made from cotton with a high-waisted rise. Measurements: 76 cm waist, 74 cm inseam, and 28 cm rise.', 'WJeans.jpg', GETDATE()),
('Trench Coat', 'Burberry', 'Womens', 'Medium', 'Clothing', 950, 'Vintage burgundy Burberry trench coat in very good condition, made from wool.', 'WCoat.jpg', GETDATE()),
('Boots', 'Marlboro Classics', 'Womens', '39', 'Footwear', 400, 'Vintage grey Marlboro Classics boots in good condition, with minor scuffs on the rubber front and slightly dirty soles. Made from a wool blend and rubber.', 'WShoes.jpg', GETDATE()),
('Retro Sunglasses', 'Unbranded', 'Womens', 'N/A', 'Accessories', 200, 'Stylish sunglasses with a curved plastic frame, comfortable nose pads, and brown tinted lenses. Includes thick arms and UV 400 protection.', 'WSun.jpg', GETDATE()),
('Retro Racer Sunglasses', 'Unbranded', 'Mens', 'N/A', 'Accessories', 190, 'Retro navy wrap-around sunglasses with a plastic frame, moulded nose pads, and black tinted lenses. Offers UV 400 protection.', 'Msun.jpg', GETDATE()),
('Contragrip Trainers', 'Salomon', 'Mens', '45', 'Footwear', 190, 'Vintage green Salomon Contragrip trainers in good condition with minor marks on the sole. Made from Gore-Tex fabric.', 'MShoes.jpg', GETDATE()),
('Varsity Jacket', 'Unbranded', 'Mens', 'Large', 'Clothing', 600, 'Varsity jacket in green, made from cotton. In good condition with a small mark on the right sleeve cuff.', 'MJacket.jpg', GETDATE()),
('Cargo Trousers', 'Wrangler', 'Mens', '38', 'Clothing', 500, 'Vintage Wrangler beige cargo trousers made from cotton. In good condition with a mark on the back. Measurements: 97 cm waist, 76 cm inseam, and 33 cm rise.', 'MCargo.jpg', GETDATE()),
('Long Sleeve T-Shirt', 'Champion', 'Mens', 'Large', 'Clothing', 800, 'Vintage white Champion long sleeve t-shirt in good condition, featuring a very faint mark on the back. Made from cotton.', '202410041656411.jpg', GETDATE()),
('Mini Dress', 'Swish Jeans', 'Womens', 'Small', 'Clothing', 550, 'Vintage black Swish Jeans mini dress in very good condition, made from a cotton blend.', '202410041656345.jpg', GETDATE());
```

### 6. Admin Access
- After registering, use the database to update your user’s role to "Admin".
- In the `AspNetUserRoles` table, manually assign the `Admin` role to your user.
- You can also use the following credentials to log in as an admin:
  - **Email**: `admin.admin@gmail.com`
  - **Password**: `admin123`

