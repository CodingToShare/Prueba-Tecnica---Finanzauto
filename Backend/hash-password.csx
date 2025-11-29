#r "nuget: BCrypt.Net-Next, 4.0.3"
using BCrypt.Net;

var adminPassword = "Admin123!";
var userPassword = "User123!";

Console.WriteLine($"Admin hash: {BCrypt.HashPassword(adminPassword, BCrypt.GenerateSalt(11))}");
Console.WriteLine($"User hash: {BCrypt.HashPassword(userPassword, BCrypt.GenerateSalt(11))}");
