using FinFlow.Application.Contracts.Users;
using FinFlow.Application.Contracts.Users.Request;
using FinFlow.Application.Contracts.Users.Response;
using FinFlow.Application.Implementation.Users.Mappers;
using FinFlow.Domain;
using FinFlow.Domain.Repositories;

namespace FinFlow.Application.Implementation.Users;

public class UserService(IUserRepository userRepository, IPasswordHasher passwordHasher, IJwtTokenService jwtTokenService) : IUserService
{
    public async Task<UserResponse> Create(UserRequest request, CancellationToken cancellationToken)
    {
        var user = await CreateInternal(request, cancellationToken);

        return UsersMappers.MapFromEntity(user);
    }

    public async Task<UserResponse> CreateUserFormAuth(CreateUserRequest request, CancellationToken cancellationToken)
    {
        var user = await CreateUserRequestInternal(request, cancellationToken);

        return UsersMappers.MapFromEntity(user);
    }

    public async Task<LoginResponse> Login(LoginRequest request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetUserByEmail(request.Email, cancellationToken);

        if (user == null)
        {
            throw new InvalidOperationException("Usuario no encontrado");
        }

        if (!passwordHasher.VerifyPassword(request.Password, user.Password))
        {
            throw new InvalidOperationException("Contraseña incorrecta");
        }

        var userResponse = UsersMappers.MapFromEntity(user);
        var token = jwtTokenService.GenerateToken(userResponse);

        return new LoginResponse(token, userResponse);
    }

    public async Task<UserResponse?> GetUser(string id, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetUser(id, cancellationToken);
        if (user == null)
        {
            return null;
        }

        return UsersMappers.MapFromEntity(user);
    }

    public async Task<UserResponse?> GetUserByAuthId(UserRequest request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetUserByAuthId(request.AuthId, cancellationToken);

        if (user != null)
        {
            return UsersMappers.MapFromEntity(user);
        }

        var newUser = await CreateInternal(request, cancellationToken);

        return UsersMappers.MapFromEntity(newUser);
    }

    private async Task<User> CreateInternal(UserRequest request, CancellationToken cancellationToken)
    {
        var user = UsersMappers.MapToDomain(request);

        await userRepository.Create(user, cancellationToken);
        return user;
    }

    private async Task<User> CreateUserRequestInternal(CreateUserRequest request, CancellationToken cancellationToken)
    {
        var user = UsersMappers.MapToDomain(request, passwordHasher);

        await userRepository.Create(user, cancellationToken);
        return user;
    }
}