# CursoLog API

CursoLog is a social-focused platform for tracking and reviewing online courses. It allows users to maintain a profile, rate courses, and share reviews, while providing a structured way to discover content across various platforms and instructors. 

## 🚀 Technologies

- **Backend:** .NET 10 & Entity Framework Core
- **Database:** PostgreSQL
- **Security:** ASP.NET Core Identity + JWT Authentication
- **Documentation:** Swagger
- **Logging:** Serilog
- **Containerization:** Docker & Docker Compose

## 🛠️ Getting Started

### Prerequisites
- [Docker Desktop](https://www.docker.com/products/docker-desktop/)

### Setup
1. Clone the repository:
   ```bash
   git clone https://github.com/pedrody/CursoLog.git
   cd CursoLog
   ```
2. Start the containers:
   ```bash
   docker compose up -d
   ```
3. The API will be available at `http://localhost:5000`.
4. To explore the API documentation, access the **Swagger UI** at: `http://localhost:5000/swagger`

### 🧪 Testing with Initial Data

The database is automatically seeded with initial data (Categories, Technologies, Platforms, Instructors, Courses, and Reviews) to facilitate testing.

You can use the following credentials to explore the API:

| Role | Email | Password |
| :--- | :--- | :--- |
| **Admin** | `admin@email.com` | `Password@123` |
| **User** | `pedro@email.com` | `Password@123` |
| **User** | `guilherme@email.com` | `Password@123` |

**Note:** These credentials are for development and testing purposes only.

## 🛣️ API Endpoints

### Authentication
- `POST /api/Auth/register` - Register a new user
- `POST /api/Auth/login` - Login and receive JWT tokens
- `POST /api/Auth/refresh-token` - Refresh an expired access token

### User Profile
- `GET /api/User/me` - Get current user profile
- `PUT /api/User/me` - Update current user profile

### Courses
- `GET /api/Course` - Get all courses
- `GET /api/Course/{id}` - Get course details
- `GET /api/Course/filterby` - Filter courses by various criteria (name, price, language, etc.)
- `POST /api/Course` - Create a new course (Admin only)
- `PUT /api/Course/{id}` - Update a course (Admin only)
- `DELETE /api/Course/{id}` - Delete a course (Admin only)

### Reviews
- `GET /api/Review/{id}` - Get a specific review
- `GET /api/Review/course/{id}` - Get all reviews for a specific course
- `GET /api/Review/user/{id}` - Get all reviews by a specific user
- `POST /api/Review` - Add a review to a course
- `PUT /api/Review/{id}` - Update your review
- `DELETE /api/Review/{id}` - Delete your review

### Categories, Instructors, Platforms & Technologies
- Full CRUD operations available for each:
  - `GET /api/[Controller]` - List all
  - `GET /api/[Controller]/{id}` - Get by ID
  - `POST`, `PUT`, `DELETE` - Management operations (Admin only)

## 🔮 Next Steps

- [ ] **Front-end Web:** Develop a modern web interface to allow users to interact with the CursoLog ecosystem.
