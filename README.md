# GeekStore API

## Description
GeekStore is an API-based platform focused on managing products, tiers and categories of computer hardware. The API allows users to interact with product data, categorize them and manage their tier classifications. It includes authentication and authorization features, enabling users with different roles to perform various operations like creating, reading, updating, and deleting (CRUD) data.

## Project Structure

This project is built using **ASP.NET Core Web API** with **Entity Framework Core** for database interaction. The following sections explain the core components of the project:

---

### **EndPoints**

The controllers handle incoming HTTP requests and interact with repositories to fetch or modify data.

#### 1. `ProductsController.cs`
- **Purpose**: Manages the CRUD operations for products in the system.
- **Key Endpoints**:
  - **POST `/api/products`**: Creates a new product (only accessible by users with the "Writer" role).
  - **GET `/api/products`**: Retrieves a list of products with filtering, sorting, and pagination options (accessible by "Reader" and "Writer" roles).
  - **GET `/api/products/{id}`**: Retrieves a product by its ID.
  - **PUT `/api/products/{id}`**: Updates an existing product.
  - **DELETE `/api/products/{id}`**: Deletes a product.

#### 2. `TiersController.cs`
- **Purpose**: Manages CRUD operations for tiers, which categorize products into "Low end", "Mid end", or "High end".
- **Key Endpoints**:
  - **POST `/api/tiers`**: Creates a new tier (only accessible by users with the "Writer" role).
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

---

### **Data Models (Domains)**

The **Domain Models** represent the entities in the application.

#### 1. `Product.cs`
- Represents a product with properties such as `Name`, `Description`, `Price`, `TierId`, and `CategoryId`.
- Includes navigation properties to link products to their respective `Tier` and `Category`.

#### 2. `Tier.cs`
- Represents a product's tier classification (e.g., "Low end", "Mid end", "High end").

#### 3. `Category.cs`
- Represents product categories like "CPU", "GPU", etc.

---

### **DTOs (Data Transfer Objects)**

DTOs define the structure of data that will be sent or received through API requests.

#### 1. `AddCategoryRequestDto.cs`
- Defines the data required to create a new category, including the category `Name`.

#### 2. `AddProductRequestDto.cs`
- Defines the data required to create a new product, including `Name`, `Description`, `Price`, `TierId`, and `CategoryId`.

#### 3. `AddTierRequestDto.cs`
- Defines the data required to create a new tier, including the tier `Name`.

#### 4. `CategoryDto.cs`
- A read-only DTO representing the `Category` model with `Id` and `Name`.

#### 5. `ProductDto.cs`
- A DTO for representing product data with properties like `Id`, `Name`, `Description`, `Price`, and associated `Tier` and `Category` objects.

#### 6. `TierDto.cs`
- A read-only DTO representing the `Tier` model with `Id` and `Name`.

#### 7. `LoginRequestDto.cs`
- Contains properties for user login: `Username` (email) and `Password`.

#### 8. `LoginResponseDto.cs`
- Represents the response to a successful login, containing the `JwtToken` for authentication.

#### 9. `RegisterRequestDto.cs`
- Contains the information required to register a new user, including `UserName`, `Password`, and `Roles`.

#### 10. `UpdateCategoryRequestDto.cs`
- Defines the data required to update a category, including the `Name`.

#### 11. `UpdateProductRequestDto.cs`
- Defines the data required to update a product, including `Name`, `Description`, `Price`, `TierId`, and `CategoryId`.

#### 12. `UpdateTierRequestDto.cs`
- Defines the data required to update a tier, including the `Name`.

---

### **Database Contexts**

#### 1. `GeekStoreAuthDbContext.cs`
- **Purpose**: Manages user authentication and roles using ASP.NET Identity.
- Includes default `Reader` and `Writer` roles with predefined GUIDs for data seeding.

#### 2. `GeekStoreDbContext.cs`
- **Purpose**: Manages product, category, and tier data.
- Includes predefined tiers ("Low end", "Mid end", "High end") and categories (e.g., "CPU", "GPU") for data seeding.

---

### **Repositories**

Repositories provide the abstraction layer for data access operations.

#### 1. `IProductRepository.cs`
- Interface for CRUD operations on `Product` entities.

#### 2. `ITierRepository.cs`
- Interface for CRUD operations on `Tier` entities.

#### 3. `ICategoryRepository.cs`
- Interface for CRUD operations on `Category` entities.

---

## Setup Instructions to clone the Repository**:
   `git clone https://github.com/yourusername/GeekStore-API.git`
