# GeoDelivery

A high-performance, production-ready Modular Monolith platform designed for modern food delivery operations, built using Domain-Driven Design (DDD) principles and clean architecture patterns in .NET.

Core Features & Functionality

Restaurant Management: Onboard and configure partner restaurants, manage operational status (active/inactive), and maintain rich profile descriptions.

Dynamic Menu & Category Organization: Structure restaurant menus into logical product categories (e.g., Appetizers, Main Courses, Beverages) with isolated aggregate relationships.

Dish & Item Control: Maintain comprehensive item catalogs with precise pricing, availability toggles, and detailed descriptions for seamless customer browsing.

Modular Extensibility: Designed with strict boundaries to easily scale out or introduce upcoming domains such as Order Processing, Real-Time Delivery Tracking, and Payment Integrations without tightly coupling business logic.

Architecture & Design

GeoDelivery avoids premature distributed microservice complexity while maintaining fully isolated business domains. Each functional module owns its persistence layer, domain logic, and application workflows, ensuring clean separation between domain models, application logic, and data access.

Tech Stack

Framework: .NET

Database & ORM: PostgreSQL, Entity Framework Core

Containerization: Docker & Docker Compose

Architecture Style: Modular Monolith with DDD patterns
