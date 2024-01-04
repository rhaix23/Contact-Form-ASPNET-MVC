# ASP.NET MVC Email Sender for Contact Forms

## Overview
The **ASP.NET MVC Email Sender for Contact Forms** is a robust and easy-to-integrate feature designed for web applications. This module efficiently handles outgoing email functionalities, typically used in contact forms. It's built with ASP.NET MVC, showcasing a clean and scalable approach to managing email communications within your web projects.

## Key Features
- **Simple Integration**: Seamlessly integrates with existing ASP.NET MVC projects, allowing for easy setup and customization.
- **Configurable SMTP Settings**: Supports customizable SMTP server settings for broad compatibility and flexibility.
- **User-friendly Contact Forms**: Includes a ready-to-use contact form example, demonstrating how to capture user input and send emails.
- **Asynchronous Sending**: Leverages asynchronous email sending, ensuring a responsive user interface and improved performance.
- **Error Handling**: Robust error handling mechanisms to ensure reliability and ease of debugging.
- **Security Focused**: Implements security best practices to protect against common vulnerabilities like SQL injection and cross-site scripting (XSS) in form inputs.

## Use Cases
This feature is ideal for website developers seeking to implement a reliable email communication system in their projects. It can be used in:
- Customer support contact forms
- Feedback collection systems
- Any web application requiring email notifications or communications

## Getting Started
To get started, simply clone the repository and follow the configuration setup instructions to integrate it into your ASP.NET MVC project.

## Configuration Setup
Ensure you have the following configuration in your `appsettings.json` file:

```json
"EmailConfiguration": {
  "Name": "Receiver name",
  "Email": "Receiver email",
  "SmtpServer": "SMTP server address",
  "SmtpPort": SMTP server port,
  "SmtpUsername": "SMTP username",
  "SmtpPassword": "SMTP password"
}
```
