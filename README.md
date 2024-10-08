## Overview

This project is a Discount Code Service that allows generating and using discount codes through a gRPC interface. The service is built using .NET and is containerized using Docker.

## Prerequisites

Before you start, ensure you have the following installed:

- [Docker](https://docs.docker.com/get-docker/)
- [Docker Compose](https://docs.docker.com/compose/install/)

## Getting Started

Follow these steps to run the Discount Code Service using Docker Compose:

### 1. Clone the Repository

Clone the project repository to your local machine:

```bash
git clone https://github.com/Lidiadev/discount-code-service.git
cd <repository-directory>
```

### 2. Set Up Environment Variables

Replace the `TBD` values in the `.env.local` file in the root of the project directory with your actual database connection string and Redis connection string:

```dotenv
DEFAULT_CONNECTION_STRING=TBD
REDIS_CONNECTION_STRING=TBD
```

### 3. Build and Run the Services

Use Docker Compose to build and run the services defined in the `docker-compose.yml` file. In the root of the project directory, run the following command:

```bash
docker-compose up --build
```

This command will:

- Build the `discountcode.service` image from the specified Dockerfile.
- Start the `discountcode.service`, `db` (PostgreSQL), and `redis` (Redis) services.

### 4. Access the Services

Once the services are running, you can access the Discount Code Service through the specified port:

- `http://localhost:5023`


