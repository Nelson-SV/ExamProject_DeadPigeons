# Døde Duer Game Management System 🎯

## 📚 Project Overview
This project is a **web application** developed for a local sports association to digitize and manage their weekly game, Døde Duer.  
The platform allows players to **purchase tickets, play the game online, and track their history**, while administrators can **manage users, payments, and game sessions** from a single dashboard.

Developed as part of a **third-semester project** at EASV, the application combines:
- **Modern web development with React (TypeScript)**.
- **A robust backend API using ASP.NET Core**.
- **Secure identity and payment management with role-based access control**.

---

## 🌐 Live Demo
👉 [Try the application here](https://deadpigeon-acfe6.web.app/)

### Test Credentials
#### Admin Account
Email: InitialUser@yahoo.com Password: BogusPassword1!

#### Player Account
Email: player@yahoo.com Password: Player1!

---

## 🛠️ Technologies Used
### Backend
- **ASP.NET Core** (Controller-based APIs)
- **PostgreSQL** (Database)
- **Docker** (Containers)
- **FluentValidation** (Validation)
- **Swagger (Swashbuckle)** (API Documentation)

### Frontend
- **React (TypeScript)** (Client-side framework)
- **TailwindCSS** (Styling)
- **daisyUI** (Components library)
- **Axios (swagger-typescript-api)** (API client generation)
- **Jotai** (State management)
- **React Router** (Routing)

### Infrastructure
- **GitHub Actions** (CI/CD)
- **Google Cloud, Aiven, Firebase** (Hosting & deployment)

---

## ✨ Key Features
- 🔹 **Players can:**
    - Buy tickets for weekly games
    - View ticket history (with indicators for winning tickets)
    - Top up account balance and track payment history

- 🔹 **Administrators can:**
    - Manage users (create, update, delete)
    - Approve or reject payments
    - Set weekly winning numbers (automatically triggers new game creation)
    - View comprehensive game and ticket overviews

- 🔐 **Security Highlights**
    - **Argon2id** password hashing with unique salts
    - **Role-based access control** to restrict admin functions
    - **JWT-based authentication** with SHA512 signing and token validation

- 📧 **Automated Email Notifications**
    - Welcome emails for new users
    - Winning ticket notifications
    - Admin alerts for pending payment approvals
