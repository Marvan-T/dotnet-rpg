# When things get COMPLEX

The beauty of Entity Framework is that it employs a convention-over-configuration paradigm, which means that it makes intelligent guesses based on the structure of your model classes, and it will often create these relationships automatically when a migration is generated.
The Fluent API configurations are essentially a way of overriding these conventions when you need to. For example, if you want to use a non-convention name for your foreign key, or specify deletion behavior, you would need to use Fluent API.
However, it is recommended to use Fluent API configurations with complex relationships or where explicit configuration is preferred over default conventions. It makes the relationships in the model clearer to anyone reading the code. Moreover, Fluent API provides more functionality compared to Data Annotations. It provides configurations for many-to-many relationships, one-to-one relationships, property types, etc., which are not achievable through Data Annotations.


