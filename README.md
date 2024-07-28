# Operations API

A less-serious/practice project to practice real domain-driven design.

## Overview

The Operations API is designed to facilitate all warehouse-operations activities related to the e-commerce project. This project serves as a practice tool to simulate basic warehouse operations, including receiving, putaways, picking, loading, and shipping.

## Features

### Technologies Used

- .NET 8
- C#
- Web API

### Architecture

- **Domain-Driven Design:** Real-world modeled domain with excellent use of encapsulation.
- **Encapsulation:**
  - Private constructors with factory pattern.
  - Internal access modifiers to keep business logic within the domain.
  - Meaningful method names, modelled to represent real-world actions.

### Functionality

- **Warehouse Operations:**
  - **Receiving:** Handles incoming goods and updates inventory.
  - **Putaways:** Manages the placement of goods into storage.
  - **Picking:** Facilitates the selection of items for orders.
  - **Loading:** Oversees the loading of goods for shipment.
  - **Shipping:** Manages the dispatch of goods to customers.
  
- **Minimal Endpoints:**
  - Uses repositories to fetch models.
  - Conducts business logic within models.

### Design Principles

- **Async Programming:** Ensures efficient and non-blocking operations.
- **Factory Pattern:** Utilizes private constructors for better encapsulation.
- **Internal Business Logic:** Keeps business logic within the domain, enhancing maintainability and security.

This project is a simple practice tool to help understand and implement domain-driven design principles in a warehouse operations context.

