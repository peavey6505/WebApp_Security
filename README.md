**Authentication & Authorization – ASP.NET (Under the Hood)**

This repository contains a learning-focused ASP.NET solution created as part of a course on **authentication and authorization.**

**Key Topics**

- Cookie vs JWT authentication
- Claims, identities, and principals
- Policy-based authorization
- ASP.NET Identity internals
- Manual vs framework-based auth
- JWT Bearer tokens



**Projects:**

**🔹 WebApp_UnderTheHood**

  Demonstrates manual cookie-based authentication without ASP.NET Identity.

Covers:
- Manual creation of Claims, ClaimsIdentity, and ClaimsPrincipal
- Signing in/out mechanisms 
- How authentication cookies are issued and used



**🔹 WebApi**

An authentication API issuing JWT Bearer tokens.

Covers:
- Claims creation
- JWT token generation and validation
- Cookie generation and validation
- Policy-based authorization



**🔹 WebApp**

Client application using **ASP.NET Identity** and **EF Core**.

Covers:
- Identity configuration and password validation
- Claims-based authentication
- Cookie-based login for browser access
- JWT authentication via HttpClient
