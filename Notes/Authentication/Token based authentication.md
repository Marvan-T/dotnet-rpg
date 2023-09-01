# Authentication with JWT tokens

## How authentication with JWT tokens work

JWT is a method for securely transmitting information between parties as a JSON object. It consists of three parts: a header, a payload, and a signature. The header contains information about the type of token and the algorithm used for signing the token. The payload contains the claims, which are statements about an entity (typically the user) and additional data. The signature is used to verify the integrity of the token.

Here's a step-by-step overview of how authentication with JWT works:

1. User Authentication: When a user tries to log in to your application, they provide their credentials (e.g., username and password) to the server.

2. Server Validation: The server validates the user's credentials. If the credentials are valid, the server generates a JWT token.

3. Token Generation: The server creates a JWT token by combining the header, payload, and signature. The header typically specifies the token type as "JWT" and the signing algorithm (e.g., HMAC, RSA). The payload contains claims such as the user's ID, username, and any other relevant information. The server signs the token using a secret key known only to the server.

4. Token Issuance: The server sends the JWT token back to the client as a response to the login request.

5. Client Storage: The client (typically a web browser or mobile app) stores the received JWT token securely. It can be stored in local storage, session storage, or a cookie, depending on the application's requirements.

6. Subsequent Requests: For subsequent requests to protected resources, the client includes the JWT token in the request. This can be done by adding the token to the request headers, typically using the "Authorization" header with the value "Bearer <token>". The server can then extract the token from the request headers.

7. Token Verification: On the server side, when a protected resource is accessed, the server verifies the JWT token's signature using the secret key. This ensures that the token has not been tampered with.

8. Access Control: After verifying the token's signature, the server can extract the claims from the payload to determine the user's identity and authorization level. Based on this information, the server grants or denies access to the requested resource.

That's a high-level overview of how authentication with JWT works. It's important to note that JWT tokens are self-contained, meaning the server doesn't need to maintain session state. The token contains all the necessary information for authentication and authorization.


## Implementation overview

This summary is based on the implemented method that is used to generate the JWT token in this project 

1. The method creates a list of claims that will be included in the token. Claims represent statements about the user (e.g., ID, username).

2. It retrieves the token value from the app settings. This value is used to create a symmetric security key, which is used for signing the JWT token.

3. The method creates a `SecurityTokenDescriptor` object to describe the details of the token. This includes the subject (claims), expiration time, and signing credentials.

4. The `JwtSecurityTokenHandler` is used to create a `SecurityToken` object based on the `SecurityTokenDescriptor`. This represents the JWT token.

5. Finally, the `JwtSecurityTokenHandler` writes the token as a string using the `WriteToken` method.

## Note regarding the `Key` that is used to sign the JWT token

The key used for signing the JWT token can be any string, but it's important to choose a strong and secure key to ensure the integrity and confidentiality of the tokens. Here are a few considerations:

1. Length: The key should have sufficient length to provide security. A key length of at least 256 bits (32 bytes) is recommended.

2. Randomness: The key should be generated using a cryptographically secure random number generator. Avoid using predictable or easily guessable values.

3. Confidentiality: Keep the key secret and don't expose it publicly. If an attacker gains access to the signing key, they can create valid tokens and potentially impersonate users.

4. Key rotation: It's good practice to periodically rotate the signing key. This helps mitigate the impact of a compromised key and improves overall security.

Additionally, you have the option to use different types of keys for signing JWT tokens, such as symmetric keys or asymmetric key pairs (public-private key). The choice depends on your specific requirements and the security mechanisms you have in place.

Remember, the security of your JWT tokens relies heavily on the strength and protection of the signing key. So, take appropriate measures to ensure its confidentiality and integrity.