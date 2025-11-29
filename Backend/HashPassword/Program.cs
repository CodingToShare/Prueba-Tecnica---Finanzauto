using System;
using BCrypt.Net;

var adminPassword = "Admin123!";
var userPassword = "User123!";

var adminHash = BCrypt.Net.BCrypt.HashPassword(adminPassword, BCrypt.Net.BCrypt.GenerateSalt(11));
var userHash = BCrypt.Net.BCrypt.HashPassword(userPassword, BCrypt.Net.BCrypt.GenerateSalt(11));

Console.WriteLine($"Admin hash: {adminHash}");
Console.WriteLine($"User hash: {userHash}");
