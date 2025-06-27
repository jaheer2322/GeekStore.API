# GeekStore API - Intelligent PC part management and recommendation API
![.NET](https://img.shields.io/badge/.NET-8.0-blue)
![ASP.NETCore](https://img.shields.io/badge/ASP.NETCore-8-orange)
![EntityFramework](https://img.shields.io/badge/EntityFrameworkCore-8.0-blue)
![PostgreSQL](https://img.shields.io/badge/PostgreSQL-17-blue)
![SQLServer](https://img.shields.io/badge/SQLServer-16-blue)
![LLM-Powered](https://img.shields.io/badge/LLMAPI-Groq-red)
![Python](https://img.shields.io/badge/Python-3-yellow)

GeekStore is a high-performance backend platform for PC part management and LLM-powered build recommendations — built with ASP.NET Core, PostgreSQL and pgvector.

<p align="center">
  <img src="./Assets/Architecture/ProductCreationCodeFlow.gif" width="35%" alt="Demo of GeekStore Product Creation" />
  <img src="./Assets/Architecture/RecommendationCodeFlow.gif" width="36%" alt="Demo of GeekStore Recommendation Flow" />
  <div align="center"><p3>(Animated explanations below)</p3></div>
</p>

## Description

GeekStore is a RESTful, API-first backend platform designed to manage products, tiers, and categories for computer hardware. It features secure authentication and role-based authorization, CRUD functionality for all core resources, and a smart recommendation engine powered by an LLM integrated via Groq API.

This project is built using **ASP.NET Core Web API** and **Entity Framework Core**, with advanced semantic similarity search using **pgvector** in PostgreSQL and AI integration through **Groq LLM API**. It cintains a dedicated Python microservice asynchronously generates embeddings, decoupled via a queue-based pipeline to ensure optimal performance and modularity.

## Tech Stack

| Component                  | Technology Used                                              |
|---------------------------|--------------------------------------------------------------|
| Backend Framework         | ASP.NET Core Web API                                         |
| ORM & Databases           | Entity Framework Core, PostgreSQL, SQL Server               |
| Vector Similarity Search  | pgvector (PostgreSQL extension)                             |
| AI/LLM Integration        | Groq LLM API                                                 |
| Authentication & Roles   | ASP.NET Identity, JWT                                        |
| DTO Mapping               | AutoMapper                                                  |
| Embedding Service         | Python (via pythonnet for embedding generation)             |
| Logging & Monitoring      | Serilog                                                     |

---

## Endpoints Overview
### Authentication Endpoints

**Purpose**: Handles user identity management.
**Database used**: SQLServer 16

| HTTP Method | Endpoint             | Description                              | Role Required |
|-------------|----------------------|------------------------------------------|----------------|
| POST        | /api/auth/register   | Register a new user with roles           | None           |
| POST        | /api/auth/login      | Authenticate user and return JWT token   | None           |

### Consumer Endpoints

**Purpose**: Controllers manage CRUD operations on different resources. Role-based access control is enforced.
**Database used**: PostgreSQL 17

**Roles**: Admin, Writer, Reader

| HTTP Method | Endpoint             | Minimum Role |
|-------------|----------------------|---------------|
| POST        | /api/resource         | Writer        |
| GET         | /api/resource/{id}    | Reader        |
| GET         | /api/resource         | Reader        | (Query is also possible with filters, more details below)
| PUT         | /api/resource/{id}    | Writer        |
| DELETE      | /api/resource/{id}    | Writer        |

---

## Recommendation Engine

A cutting-edge feature that helps users build optimized PC configurations using **Semantic search** and **LLM Integration**.

### How It Works

[Product Creation Codeflow]
![Product Creation Flow](./Assets/Architecture/ProductCreationCodeFlow.gif)

1. **Product Creation & Embedding**
   - When a new product is added, it is saved to the PostgreSQL and returns a response immediately.
   - Upon successful creation of a product, an asynchronous embedding request is queued to generate embedding for the product via `IEmbeddingService`.
   - The resulting vector is stored in PostgreSQL using the **pgvector** extension alongside the created product data.

[Recommendation Codeflow]
![Recommendation Flow](./Assets/Architecture/RecommendationCodeFlow.gif)

2. **User Query**
   - The user sends a plain-text requirement like:  
     *"I need a gaming PC under 1 Lakh rupees with a strong GPU."*

3. **Query Embedding**
   - The input query is passed to a Python-based microservice (`IEmbeddingService`) that generates a semantic vector.

4. **Similarity Search**
   - A **cosine similarity search** is performed on the stored product vectors (via `pgvector`) using Entity Framework + PostgreSQL.

5. **Top Matches Per Category**
   - The backend retrieves the **top 10 (CPU, Motherboard, GPU) & 5 (Storage, RAM, etc.) most relevant products per category**. Also implements low confidence rejection for accurate results.

6. **LLM-Powered Optimization**
   - The shortlisted products and the original query along with a system prompt are sent to a reasoning LLM (via `ILLMService`).
   - The system uses strict rules in the prompt to ensure consistent output format and accuracy.
   - The LLM returns **two optimized PC builds** in the form of category-product ID mappings.
   - The engine implements graceful fallback messaging, ensuring clear and user-friendly responses even when no valid recommendations are found.

7. **DTO Mapping & Final Output**
   - The recommended product IDs are mapped to full product details.
   - Two complete build recommendations are sent back to the client as `RecommendationsDto`.
   - Explanation containing the thought process behind each decision. (If `isExplanationNeeded` set to true in user query)

### Performance & Scalability

- The engine is built to handle **millions of products** thanks to:
  - **PostgreSQL + pgvector**, optimized for fast vector similarity search even with high-dimensional data.
  - Efficient indexing via **HNSW**, ensuring sub-millisecond query times.
  - Stateless services and modular design — ideal for horizontal scaling under heavy load.
- **LLM processing and embedding are fully offloaded**, keeping the API lightweight and performant under concurrent usage.
- **Real-time recommendations stay fast** — even as the product catalog grows exponentially.
- **Strict Prompting Strategy** — Groq LLM is guided with precise rules—returning only valid IDs, limiting to two results, and prioritizing accuracy. This ensures structured and reliable outputs.
- **Fallback for No Match Scenarios** — If no high-confidence match is found, the system gracefully responds with a clear, fallback message—avoiding forced or irrelevant suggestions.
- **Asynchronous Embedding Pipeline** — Embedding generation is fully decoupled and handled in the background using a task queue, boosting throughput without blocking user-facing endpoints.
- **Optimized DTO Usage:** Reduces payload size and boosts performance with AutoMapper.
- **Logging with Serilog:** Logs API activity to both console and persistent storage.
- **Scalable Storage:** PostgreSQL and SQL Server integration support high data volume and fast access.
- **Modular Architecture:** Follows clean coding practices with separation of controller, service, and repository layers.

### Example Query

- Request: I need a PC for video editing and occasional gaming under ₹80,000.
- Response: Returns two build recommendations optimized for **editing + budget gaming**, using the most relevant components available.

---
## Endpoints Elaborated

| Controller         | Endpoint               | Method | Role Required | Description                                 |
| ------------------ | ---------------------- | ------ | ------------- | ------------------------------------------- |
| **Auth**           | `/api/auth/register`   | POST   | None          | Register a new user                         |
|                    | `/api/auth/login`      | POST   | None          | Authenticate and receive JWT token          |
| **Products**       | `/api/products`        | GET    | Reader        | Get all products (with filters/sorting)     |
|                    | `/api/products/{id}`   | GET    | Reader        | Get product by ID                           |
|                    | `/api/products`        | POST   | Writer        | Create a new product                        |
|                    | `/api/products/{id}`   | PUT    | Writer        | Update product by ID                        |
|                    | `/api/products/{id}`   | DELETE | Writer        | Delete product by ID                        |
| **Tiers**          | `/api/tiers`           | GET    | Reader        | Get all tiers                               |
|                    | `/api/tiers/{id}`      | GET    | Reader        | Get tier by ID                              |
|                    | `/api/tiers`           | POST   | Writer        | Create a new tier                           |
|                    | `/api/tiers/{id}`      | PUT    | Writer        | Update tier by ID                           |
|                    | `/api/tiers/{id}`      | DELETE | Writer        | Delete tier by ID                           |
| **Categories**     | `/api/categories`      | GET    | Reader        | Get all categories                          |
|                    | `/api/categories/{id}` | GET    | Reader        | Get category by ID                          |
|                    | `/api/categories`      | POST   | Writer        | Create a new category                       |
|                    | `/api/categories/{id}` | PUT    | Writer        | Update category by ID                       |
|                    | `/api/categories/{id}` | DELETE | Writer        | Delete category by ID                       |
| **Recommendation** | `/api/recommendations` | POST   | Reader        | Generate PC build recommendations (via LLM) |

#### Request bodies
##### POST `/products`

###### ➤ Example Request Body

```json
{
  "name": "Corsair Vengeance RGB 32 GB DDR5-6000",
  "price": 12999.99,
  "description": "High-performance DDR5 RAM with dynamic RGB lighting and tight timings for gaming and multitasking.",
  "review": "Fantastic value for gamers. Solid performance with great overclocking headroom.",
  "tierId": "b6e2313b-3f46-4e76-9db0-22993c3086ab",
  "categoryId": "f5c5ab60-64bb-4bb0-b271-8b8a9b5d83c7"
}
```

###### Validation Rules

| Field         | Type    | Constraints                                 |
|---------------|---------|---------------------------------------------|
| `name`        | string  | Required, 1–100 characters                  |
| `price`       | double  | Required, must be between 1 and 1,000,000   |
| `description` | string  | Optional, 1–1000 characters                 |
| `review`      | string  | Optional, 10–5000 characters                |
| `tierId`      | GUID    | Required                                    |
| `categoryId`  | GUID    | Required                                    |

---

###### POST `/recommend`

###### ➤ Request Body

```json
{
  "query": "Build me a high-end gaming PC for 4K gaming and streaming",
  "isExplanationNeeded": true
}
```

###### Validation Rules

| Field               | Type    | Constraints                          |
|---------------------|---------|--------------------------------------|
| `query`             | string  | Required, minimum 24 characters     |
| `isExplanationNeeded` | boolean | Optional          |

#### Endpoints screeshot in swagger
![Swagger endpoints full](./Assets/Screenshots/SwaggerEndpointsFull.jpg))

#### Product Listing Features

- **Filtering** on fields: `name`, `tier`, `category`
- **Sorting** on fields: `name`, `tier`, `category`
- **Pagination** with customizable page size
- **Strict parameter validation**
- **Role-based access** (`Reader`, `Writer`, `Admin`)

#### Example Request

GET /api/products?filterOn=category&filterQuery=GPU&sortBy=tier&isAscending=false&pageNumber=1&pageSize=10

#### Query Parameters

| Parameter     | Type   | Description                                                                 |
|---------------|--------|-----------------------------------------------------------------------------|
| `filterOn`    | string | Field to filter on (`name`, `tier`, `category`)                             |
| `filterQuery` | string | Value to search for in the specified `filterOn` field (Any for name, complete name for category and tier)                    |
| `sortBy`      | string | Field to sort by (`name`, `tier`, `category`)                               |
| `isAscending` | bool   | `true` for ascending, `false` for descending                                |
| `pageNumber`  | int    | Page number (starts from 1)                                                 |
| `pageSize`    | int    | Number of results per page (default: 1000, ideal for large datasets)        |

#### Swagger Screenshot of product query features
![Product Query Features](./Assets/Screenshots/ProductQueryFeatures.jpg)

### Performance & Scalability

- Supports **millions of products**
- Utilizes **PostgreSQL + indexes** for blazing fast queries
- Designed to scale under **high concurrent loads**

---

## QuickStart

### Follow these steps to get GeekStore API up and running locally:

### 1. Clone the Repository
- git clone https://github.com/your-username/GeekStore.git
- cd GeekStore
### 2. Setup Environment Variables
#### Create a .env file at the root of the project with the following keys:

<details> <summary><strong>📄 .env Example</strong> (click to expand)</summary>

#### #PostgreSQL for Product Catalog
GeekStoreConnectionString=Host=localhost;Port=5432;Database=GeekStoreDb;Username=your_username;Password=your_password

#### #SQL Server or PostgreSQL for Auth Database
GeekStoreAuthDbConnectionString=Server=localhost;Database=GeekStoreAuthDb;Trusted_Connection=True;TrustServerCertificate=True

#### #JWT Configuration
JWT_Key=your_super_secret_key
JWT_Issuer=https://localhost:7016/
JWT_Audience=https://localhost:7016/

#### #Python Embedding Service (if using pythonnet)
PythonDLLPath=C:/Path/To/pythonXY.dll
PythonScriptsFolder=C:/Path/To/GeekStore/Python

#### #Groq AI Configuration
GroqApiKey=your_groq_api_key
GroqLLMModel=your_groq_llm_key
</details>

## 3. Apply Migrations
#### Navigate to the API project directory
- cd GeekStore.API

#### Apply migrations to both databases
- dotnet ef database update --context GeekStoreDbContext
- dotnet ef database update --context GeekStoreAuthDbContext

## 4. Run the Project
- dotnet run --project GeekStore.API

## 5. Test with Swagger
- Visit: https://localhost:1234/swagger
