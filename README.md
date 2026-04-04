
# SchemaStar

A full-stack knowledge management system application for visualizing complex data relationships using an interactive graph interface.

## TechStack
**Frontend:** Angular, TypeScript, Tailwind CSS, Foblex (for graph visualization)

**Backend:** ASP.NET Core, C#, Entity Framework Core 

**Database:** MySQL 

**Testing:** Jasmine (Frontend), xUnit (Backend)

## Architecture & Security (BFF Pattern)
SchemaStar implements a **Backend-for-Frontend (BFF)** pattern to bridge the Angular client and the ASP.NET Core API. This architecture enhances security by centralizing authentication logic on the server side. I utilized **Microsoft Identity** to manage a dual-layer authentication flow: **cookie-based** auth provides a secure session for the web client, while **JWT Bearer tokens** support future cross-platform integration (Android SDK). This setup mitigates common risks like **XSS** and **CSRF** attacks by ensuring sensitive tokens are handled securely.

## Core Dependencies
**Frontend:** Angular 21, TypeScript 5.9, Node.js 24

**Backend:** .NET 10, C# 14
