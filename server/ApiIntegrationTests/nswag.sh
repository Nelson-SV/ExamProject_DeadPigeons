dotnet tool install -g NSwag.ConsoleCore

nswag openapi2csclient /input:http://localhost:5000/swagger/v1/swagger.json /output:SwaggerTest.cs /namespace:Generated /wrapResponses:true