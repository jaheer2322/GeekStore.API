# GeekStore API

## Description
GeekStore is an API-based platform focused on managing products, tiers and categories of computer hardware. The API allows users to interact with product data, categorize them and manage their tier classifications. It includes authentication and authorization features, enabling users with different roles to perform various operations like creating, reading, updating, and deleting (CRUD) data.

## Project Structure

This project is built using **ASP.NET Core Web API** with **Entity Framework Core** for database interaction. The following sections explain the core components of the project:

---

### **Authentication EndPoints**




Purpose: Manages user authentication and authorization tasks such as user registration and login.


### **Consumer EndPoints**

The controllers handle incoming HTTP requests and interact with repositories to fetch or modify data.
**Roles**: Admin, Writer, Reader
#### `Overview`
| *HTTP Method* | *Endpoint*         | *Minimum Role* |
|-------------|------------------|---------------|
| POST        | /resource        | Writer        |
| GET         | /resource/{id}   | Reader        |
| GET         | /resource        | Reader        |
| PUT         | /resource/{id}   | Writer        |
| DELETE      | /resource/{id}   | Writer        |

#### 1. `ProductsController.cs`
- **Purpose**: Manages the CRUD operations for products in the system.
- **Key Endpoints**:
  - **POST `/api/products`**: Creates a new product (Least role required is Writer).
  - **GET `/api/products`**: Retrieves a list of products with filtering, sorting, and pagination options. (Least role required is Reader).
  - **GET `/api/products/{id}`**: Retrieves a product by its ID.
  - **PUT `/api/products/{id}`**: Updates an existing product. (Least role required is Writer)
  - **DELETE `/api/products/{id}`**: Deletes a product. (Least role required is Writer)

#### 2. `TiersController.cs`
- **Purpose**: Manages CRUD operations for tiers, which categorize products into "Low end", "Mid end", or "High end".
- **Key Endpoints**:
  - **POST `/api/tiers`**: Creates a new tier (Least role required is Writer).
  - **GET `/api/tiers`**: Retrieves all available tiers.
  - **GET `/api/tiers/{id}`**: Retrieves a specific tier by its ID.
  - **PUT `/api/tiers/{id}`**: Updates a tier.
  - **DELETE `/api/tiers/{id}`**: Deletes a tier.

#### 3. `CategoriesController.cs`
- **Purpose**: Manages CRUD operations for categories, such as "CPU", "GPU", etc., which classify products.
- **Key Endpoints**:
  - **POST `/api/categories`**: Creates a new category.
  - **GET `/api/categories`**: Retrieves all categories.
  - **GET `/api/categories/{id}`**: Retrieves a specific category by its ID.
  - **PUT `/api/categories/{id}`**: Updates a category.
  - **DELETE `/api/categories/{id}`**: Deletes a category.

#### 4. `AuthController.cs`
- **Purpose**: Handles authentication and authorization for the system.
- **Key Endpoints**:
  - **POST `/api/auth/register`**: Registers a new user with assigned roles.
  - **POST `/api/auth/login`**: Logs in an existing user and generates a JWT token for authentication.
