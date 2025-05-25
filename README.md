# GeekStore API

![Demo of GeekStore Recommendation Flow](./Assets/Architecture/ProductCreationCodeFlow.gif)

## Description

GeekStore is an API-first backend platform designed to manage products, tiers, and categories for computer hardware. It features secure authentication and role-based authorization, CRUD functionality for all core resources, and a smart recommendation engine powered by an LLM integrated via Groq API.

This project is built using **ASP.NET Core Web API** and **Entity Framework Core**, with advanced vector similarity search using **pgvector** in PostgreSQL and AI integration through **Groq LLM API**. It also includes a Python microservice for **embedding generation**, keeping architecture modular and clean.

---

## Contents

- Authentication Endpoints
- Consumer Endpoints (Products, Categories, Tiers)
- Recommendation Engine (AI-Powered)
- Animated Architecture Flow

---

## Authentication Endpoints

Handles user identity management.

| HTTP Method | Endpoint             | Description                              | Role Required |
|-------------|----------------------|------------------------------------------|----------------|
| POST        | /api/auth/register   | Register a new user with roles           | None           |
| POST        | /api/auth/login      | Authenticate user and return JWT token   | None           |

---

## Consumer Endpoints

Controllers manage CRUD operations on different resources. Role-based access control is enforced.

**Roles**: Admin, Writer, Reader

| HTTP Method | Endpoint             | Minimum Role |
|-------------|----------------------|---------------|
| POST        | /api/resource         | Writer        |
| GET         | /api/resource/{id}    | Reader        |
| GET         | /api/resource         | Reader        |
| PUT         | /api/resource/{id}    | Writer        |
| DELETE      | /api/resource/{id}    | Writer        |

### 1. ProductsController
- **Purpose**: Handles product-related operations.
- **Key Endpoints**:
  - `POST /api/products`
  - `GET /api/products`
  - `GET /api/products/{id}`
  - `PUT /api/products/{id}`
  - `DELETE /api/products/{id}`

### 2. TiersController
- **Purpose**: Manages tiers like Low-end, Mid-end, and High-end.
- **Key Endpoints**:
  - `POST /api/tiers`
  - `GET /api/tiers`
  - `GET /api/tiers/{id}`
  - `PUT /api/tiers/{id}`
  - `DELETE /api/tiers/{id}`

### 3. CategoriesController
- **Purpose**: Handles component categories like CPU, GPU, etc.
- **Key Endpoints**:
  - `POST /api/categories`
  - `GET /api/categories`
  - `GET /api/categories/{id}`
  - `PUT /api/categories/{id}`
  - `DELETE /api/categories/{id}`

### 4. AuthController
- **Purpose**: Manages user authentication and role assignment.
- **Key Endpoints**:
  - `POST /api/auth/register`
  - `POST /api/auth/login`

---

## Recommendation Engine

A cutting-edge feature that helps users build optimized PC configurations using natural language queries.

### How it Works

1. The user provides a natural language input (e.g., *"I need a gaming PC under 1 Lakh rupees with a strong GPU"*).
2. The query is embedded using a Python microservice via the `IEmbeddingService`.
3. The embedded vector is used for a **cosine similarity search** using `pgvector` in PostgreSQL.
4. Top similar products are selected across each category.
5. The list is passed to a **Groq LLM**, which returns **two optimized build configurations** (as product ID lists).
6. Final product details are fetched and returned as a complete PC build recommendation.

---

## Animated Architecture Codeflow

### Product Creation Flow
![Product Creation Codeflow](./Assets/Architecture/ProductCreationCodeFlow.gif)

---

