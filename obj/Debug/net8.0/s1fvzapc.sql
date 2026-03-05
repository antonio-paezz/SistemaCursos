IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

CREATE TABLE [AspNetRoles] (
    [Id] nvarchar(450) NOT NULL,
    [Name] nvarchar(256) NULL,
    [NormalizedName] nvarchar(256) NULL,
    [ConcurrencyStamp] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetRoles] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [AspNetUsers] (
    [Id] nvarchar(450) NOT NULL,
    [UserName] nvarchar(256) NULL,
    [NormalizedUserName] nvarchar(256) NULL,
    [Email] nvarchar(256) NULL,
    [NormalizedEmail] nvarchar(256) NULL,
    [EmailConfirmed] bit NOT NULL,
    [PasswordHash] nvarchar(max) NULL,
    [SecurityStamp] nvarchar(max) NULL,
    [ConcurrencyStamp] nvarchar(max) NULL,
    [PhoneNumber] nvarchar(max) NULL,
    [PhoneNumberConfirmed] bit NOT NULL,
    [TwoFactorEnabled] bit NOT NULL,
    [LockoutEnd] datetimeoffset NULL,
    [LockoutEnabled] bit NOT NULL,
    [AccessFailedCount] int NOT NULL,
    CONSTRAINT [PK_AspNetUsers] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [AspNetRoleClaims] (
    [Id] int NOT NULL IDENTITY,
    [RoleId] nvarchar(450) NOT NULL,
    [ClaimType] nvarchar(max) NULL,
    [ClaimValue] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetRoleClaims] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [AspNetUserClaims] (
    [Id] int NOT NULL IDENTITY,
    [UserId] nvarchar(450) NOT NULL,
    [ClaimType] nvarchar(max) NULL,
    [ClaimValue] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetUserClaims] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [AspNetUserLogins] (
    [LoginProvider] nvarchar(128) NOT NULL,
    [ProviderKey] nvarchar(128) NOT NULL,
    [ProviderDisplayName] nvarchar(max) NULL,
    [UserId] nvarchar(450) NOT NULL,
    CONSTRAINT [PK_AspNetUserLogins] PRIMARY KEY ([LoginProvider], [ProviderKey]),
    CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [AspNetUserRoles] (
    [UserId] nvarchar(450) NOT NULL,
    [RoleId] nvarchar(450) NOT NULL,
    CONSTRAINT [PK_AspNetUserRoles] PRIMARY KEY ([UserId], [RoleId]),
    CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [AspNetUserTokens] (
    [UserId] nvarchar(450) NOT NULL,
    [LoginProvider] nvarchar(128) NOT NULL,
    [Name] nvarchar(128) NOT NULL,
    [Value] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetUserTokens] PRIMARY KEY ([UserId], [LoginProvider], [Name]),
    CONSTRAINT [FK_AspNetUserTokens_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO

CREATE INDEX [IX_AspNetRoleClaims_RoleId] ON [AspNetRoleClaims] ([RoleId]);
GO

CREATE UNIQUE INDEX [RoleNameIndex] ON [AspNetRoles] ([NormalizedName]) WHERE [NormalizedName] IS NOT NULL;
GO

CREATE INDEX [IX_AspNetUserClaims_UserId] ON [AspNetUserClaims] ([UserId]);
GO

CREATE INDEX [IX_AspNetUserLogins_UserId] ON [AspNetUserLogins] ([UserId]);
GO

CREATE INDEX [IX_AspNetUserRoles_RoleId] ON [AspNetUserRoles] ([RoleId]);
GO

CREATE INDEX [EmailIndex] ON [AspNetUsers] ([NormalizedEmail]);
GO

CREATE UNIQUE INDEX [UserNameIndex] ON [AspNetUsers] ([NormalizedUserName]) WHERE [NormalizedUserName] IS NOT NULL;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'00000000000000_CreateIdentitySchema', N'8.0.23');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20251115213503_Inicial', N'8.0.23');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

CREATE TABLE [Categoria] (
    [Id] int NOT NULL IDENTITY,
    [Nombre] nvarchar(80) NOT NULL,
    [Descripcion] nvarchar(200) NOT NULL,
    CONSTRAINT [PK_Categoria] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Compra] (
    [Id] int NOT NULL IDENTITY,
    [FechaCompra] date NOT NULL,
    [Total] decimal(18,2) NOT NULL,
    [IdUsuario] nvarchar(max) NOT NULL,
    [UsuarioId] nvarchar(450) NULL,
    CONSTRAINT [PK_Compra] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Compra_AspNetUsers_UsuarioId] FOREIGN KEY ([UsuarioId]) REFERENCES [AspNetUsers] ([Id])
);
GO

CREATE TABLE [Curso] (
    [Id] int NOT NULL IDENTITY,
    [Titulo] nvarchar(100) NOT NULL,
    [Descripcion] nvarchar(500) NOT NULL,
    [Precio] decimal(18,2) NOT NULL,
    [FechaPublicacion] date NOT NULL,
    [IdInstructor] nvarchar(max) NOT NULL,
    [InstructorId] nvarchar(450) NULL,
    [IdCategoria] int NOT NULL,
    [CategoriaId] int NOT NULL,
    CONSTRAINT [PK_Curso] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Curso_AspNetUsers_InstructorId] FOREIGN KEY ([InstructorId]) REFERENCES [AspNetUsers] ([Id]),
    CONSTRAINT [FK_Curso_Categoria_CategoriaId] FOREIGN KEY ([CategoriaId]) REFERENCES [Categoria] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [Calificacion] (
    [Id] int NOT NULL IDENTITY,
    [Puntuacion] int NOT NULL,
    [Comentario] nvarchar(150) NOT NULL,
    [Fecha] date NOT NULL,
    [IdUsuario] nvarchar(max) NOT NULL,
    [UsuarioId] nvarchar(450) NULL,
    [IdCurso] int NOT NULL,
    [CursoId] int NOT NULL,
    CONSTRAINT [PK_Calificacion] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Calificacion_AspNetUsers_UsuarioId] FOREIGN KEY ([UsuarioId]) REFERENCES [AspNetUsers] ([Id]),
    CONSTRAINT [FK_Calificacion_Curso_CursoId] FOREIGN KEY ([CursoId]) REFERENCES [Curso] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [DetalleCompra] (
    [Id] int NOT NULL IDENTITY,
    [PrecioUnitario] decimal(18,2) NOT NULL,
    [IdCompra] int NOT NULL,
    [CompraId] int NOT NULL,
    [IdCurso] int NOT NULL,
    [CursoId] int NOT NULL,
    CONSTRAINT [PK_DetalleCompra] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_DetalleCompra_Compra_CompraId] FOREIGN KEY ([CompraId]) REFERENCES [Compra] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_DetalleCompra_Curso_CursoId] FOREIGN KEY ([CursoId]) REFERENCES [Curso] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [Inscripcion] (
    [Id] int NOT NULL IDENTITY,
    [FechaInscripcion] date NOT NULL,
    [Estado] int NOT NULL,
    [IdUsuario] nvarchar(max) NOT NULL,
    [UsuarioId] nvarchar(450) NULL,
    [IdCurso] int NOT NULL,
    [CursoId] int NOT NULL,
    CONSTRAINT [PK_Inscripcion] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Inscripcion_AspNetUsers_UsuarioId] FOREIGN KEY ([UsuarioId]) REFERENCES [AspNetUsers] ([Id]),
    CONSTRAINT [FK_Inscripcion_Curso_CursoId] FOREIGN KEY ([CursoId]) REFERENCES [Curso] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [Leccion] (
    [Id] int NOT NULL IDENTITY,
    [Titulo] nvarchar(150) NOT NULL,
    [Descripcion] nvarchar(500) NOT NULL,
    [IdCurso] int NOT NULL,
    [CursoId] int NOT NULL,
    CONSTRAINT [PK_Leccion] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Leccion_Curso_CursoId] FOREIGN KEY ([CursoId]) REFERENCES [Curso] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [Archivo] (
    [Id] int NOT NULL IDENTITY,
    [NombreArchivo] nvarchar(150) NOT NULL,
    [Ruta] nvarchar(max) NOT NULL,
    [Tipo] nvarchar(20) NOT NULL,
    [FechaSubida] date NOT NULL,
    [IdLeccion] int NOT NULL,
    [LeccionId] int NOT NULL,
    CONSTRAINT [PK_Archivo] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Archivo_Leccion_LeccionId] FOREIGN KEY ([LeccionId]) REFERENCES [Leccion] ([Id]) ON DELETE CASCADE
);
GO

CREATE INDEX [IX_Archivo_LeccionId] ON [Archivo] ([LeccionId]);
GO

CREATE INDEX [IX_Calificacion_CursoId] ON [Calificacion] ([CursoId]);
GO

CREATE INDEX [IX_Calificacion_UsuarioId] ON [Calificacion] ([UsuarioId]);
GO

CREATE INDEX [IX_Compra_UsuarioId] ON [Compra] ([UsuarioId]);
GO

CREATE INDEX [IX_Curso_CategoriaId] ON [Curso] ([CategoriaId]);
GO

CREATE INDEX [IX_Curso_InstructorId] ON [Curso] ([InstructorId]);
GO

CREATE INDEX [IX_DetalleCompra_CompraId] ON [DetalleCompra] ([CompraId]);
GO

CREATE INDEX [IX_DetalleCompra_CursoId] ON [DetalleCompra] ([CursoId]);
GO

CREATE INDEX [IX_Inscripcion_CursoId] ON [Inscripcion] ([CursoId]);
GO

CREATE INDEX [IX_Inscripcion_UsuarioId] ON [Inscripcion] ([UsuarioId]);
GO

CREATE INDEX [IX_Leccion_CursoId] ON [Leccion] ([CursoId]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20251115215107_PrimeraMigracion', N'8.0.23');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [AspNetUsers] ADD [Apellido] nvarchar(max) NOT NULL DEFAULT N'';
GO

ALTER TABLE [AspNetUsers] ADD [Ciudad] nvarchar(max) NOT NULL DEFAULT N'';
GO

ALTER TABLE [AspNetUsers] ADD [Nombre] nvarchar(max) NOT NULL DEFAULT N'';
GO

ALTER TABLE [AspNetUsers] ADD [Pais] nvarchar(max) NOT NULL DEFAULT N'';
GO

ALTER TABLE [AspNetUsers] ADD [Provincia] nvarchar(max) NOT NULL DEFAULT N'';
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20251115230509_CamposAgregadosAUsuario', N'8.0.23');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [Curso] DROP CONSTRAINT [FK_Curso_AspNetUsers_InstructorId];
GO

DECLARE @var0 sysname;
SELECT @var0 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Curso]') AND [c].[name] = N'IdInstructor');
IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [Curso] DROP CONSTRAINT [' + @var0 + '];');
ALTER TABLE [Curso] DROP COLUMN [IdInstructor];
GO

DROP INDEX [IX_Curso_InstructorId] ON [Curso];
DECLARE @var1 sysname;
SELECT @var1 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Curso]') AND [c].[name] = N'InstructorId');
IF @var1 IS NOT NULL EXEC(N'ALTER TABLE [Curso] DROP CONSTRAINT [' + @var1 + '];');
UPDATE [Curso] SET [InstructorId] = N'' WHERE [InstructorId] IS NULL;
ALTER TABLE [Curso] ALTER COLUMN [InstructorId] nvarchar(450) NOT NULL;
ALTER TABLE [Curso] ADD DEFAULT N'' FOR [InstructorId];
CREATE INDEX [IX_Curso_InstructorId] ON [Curso] ([InstructorId]);
GO

ALTER TABLE [Curso] ADD CONSTRAINT [FK_Curso_AspNetUsers_InstructorId] FOREIGN KEY ([InstructorId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE;
GO

DECLARE @var2 sysname;
SELECT @var2 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Curso]') AND [c].[name] = N'IdCategoria');
IF @var2 IS NOT NULL EXEC(N'ALTER TABLE [Curso] DROP CONSTRAINT [' + @var2 + '];');
ALTER TABLE [Curso] DROP COLUMN [IdCategoria];
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20251116201300_CorreccionRelacionesCategoriaInstructor', N'8.0.23');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [Leccion] DROP CONSTRAINT [FK_Leccion_Curso_CursoId];
GO

DECLARE @var3 sysname;
SELECT @var3 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Leccion]') AND [c].[name] = N'IdCurso');
IF @var3 IS NOT NULL EXEC(N'ALTER TABLE [Leccion] DROP CONSTRAINT [' + @var3 + '];');
ALTER TABLE [Leccion] DROP COLUMN [IdCurso];
GO

DROP INDEX [IX_Leccion_CursoId] ON [Leccion];
DECLARE @var4 sysname;
SELECT @var4 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Leccion]') AND [c].[name] = N'CursoId');
IF @var4 IS NOT NULL EXEC(N'ALTER TABLE [Leccion] DROP CONSTRAINT [' + @var4 + '];');
ALTER TABLE [Leccion] ALTER COLUMN [CursoId] int NOT NULL;
CREATE INDEX [IX_Leccion_CursoId] ON [Leccion] ([CursoId]);
GO

ALTER TABLE [Leccion] ADD CONSTRAINT [FK_Leccion_Curso_CursoId] FOREIGN KEY ([CursoId]) REFERENCES [Curso] ([Id]) ON DELETE CASCADE;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20251127002354_CorreccionRelacionCursoLeccion', N'8.0.23');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [Archivo] DROP CONSTRAINT [FK_Archivo_Leccion_LeccionId];
GO

DECLARE @var5 sysname;
SELECT @var5 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Archivo]') AND [c].[name] = N'IdLeccion');
IF @var5 IS NOT NULL EXEC(N'ALTER TABLE [Archivo] DROP CONSTRAINT [' + @var5 + '];');
ALTER TABLE [Archivo] DROP COLUMN [IdLeccion];
GO

DROP INDEX [IX_Archivo_LeccionId] ON [Archivo];
DECLARE @var6 sysname;
SELECT @var6 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Archivo]') AND [c].[name] = N'LeccionId');
IF @var6 IS NOT NULL EXEC(N'ALTER TABLE [Archivo] DROP CONSTRAINT [' + @var6 + '];');
ALTER TABLE [Archivo] ALTER COLUMN [LeccionId] int NOT NULL;
CREATE INDEX [IX_Archivo_LeccionId] ON [Archivo] ([LeccionId]);
GO

ALTER TABLE [Archivo] ADD CONSTRAINT [FK_Archivo_Leccion_LeccionId] FOREIGN KEY ([LeccionId]) REFERENCES [Leccion] ([Id]) ON DELETE CASCADE;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20251127131923_CorreccionRelacionLeccionArchivo', N'8.0.23');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

DROP TABLE [Archivo];
GO

CREATE TABLE [ContenidosLeccion] (
    [Id] int NOT NULL IDENTITY,
    [LeccionId] int NOT NULL,
    [Tipo] nvarchar(max) NULL,
    [Texto] nvarchar(max) NULL,
    [UrlArchivo] nvarchar(max) NULL,
    [Orden] int NOT NULL,
    CONSTRAINT [PK_ContenidosLeccion] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_ContenidosLeccion_Leccion_LeccionId] FOREIGN KEY ([LeccionId]) REFERENCES [Leccion] ([Id]) ON DELETE CASCADE
);
GO

CREATE INDEX [IX_ContenidosLeccion_LeccionId] ON [ContenidosLeccion] ([LeccionId]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20251127201258_AgregarContenidoLeccion', N'8.0.23');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

DECLARE @var7 sysname;
SELECT @var7 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ContenidosLeccion]') AND [c].[name] = N'UrlArchivo');
IF @var7 IS NOT NULL EXEC(N'ALTER TABLE [ContenidosLeccion] DROP CONSTRAINT [' + @var7 + '];');
ALTER TABLE [ContenidosLeccion] ALTER COLUMN [UrlArchivo] nvarchar(max) NULL;
GO

DECLARE @var8 sysname;
SELECT @var8 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ContenidosLeccion]') AND [c].[name] = N'Texto');
IF @var8 IS NOT NULL EXEC(N'ALTER TABLE [ContenidosLeccion] DROP CONSTRAINT [' + @var8 + '];');
ALTER TABLE [ContenidosLeccion] ALTER COLUMN [Texto] nvarchar(max) NULL;
GO

ALTER TABLE [ContenidosLeccion] ADD [PublicId] nvarchar(max) NULL;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20251128030349_AgregarPublicIdAContenidoLeccion', N'8.0.23');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

DECLARE @var9 sysname;
SELECT @var9 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ContenidosLeccion]') AND [c].[name] = N'Tipo');
IF @var9 IS NOT NULL EXEC(N'ALTER TABLE [ContenidosLeccion] DROP CONSTRAINT [' + @var9 + '];');
UPDATE [ContenidosLeccion] SET [Tipo] = N'' WHERE [Tipo] IS NULL;
ALTER TABLE [ContenidosLeccion] ALTER COLUMN [Tipo] nvarchar(max) NOT NULL;
ALTER TABLE [ContenidosLeccion] ADD DEFAULT N'' FOR [Tipo];
GO

CREATE TABLE [Carritos] (
    [Id] int NOT NULL IDENTITY,
    [UsuarioId] int NOT NULL,
    CONSTRAINT [PK_Carritos] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [CarritoProductos] (
    [Id] int NOT NULL IDENTITY,
    [CarritoId] int NOT NULL,
    [CursoId] int NOT NULL,
    [Cantidad] int NOT NULL,
    CONSTRAINT [PK_CarritoProductos] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_CarritoProductos_Carritos_CarritoId] FOREIGN KEY ([CarritoId]) REFERENCES [Carritos] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_CarritoProductos_Curso_CursoId] FOREIGN KEY ([CursoId]) REFERENCES [Curso] ([Id]) ON DELETE CASCADE
);
GO

CREATE INDEX [IX_CarritoProductos_CarritoId] ON [CarritoProductos] ([CarritoId]);
GO

CREATE INDEX [IX_CarritoProductos_CursoId] ON [CarritoProductos] ([CursoId]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20251207220718_AgregarCarrito', N'8.0.23');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

DECLARE @var10 sysname;
SELECT @var10 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Carritos]') AND [c].[name] = N'UsuarioId');
IF @var10 IS NOT NULL EXEC(N'ALTER TABLE [Carritos] DROP CONSTRAINT [' + @var10 + '];');
ALTER TABLE [Carritos] ALTER COLUMN [UsuarioId] nvarchar(max) NOT NULL;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20251207224242_CambiarTipoUsuarioIdCarrito', N'8.0.23');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [Curso] ADD [Estado] int NOT NULL DEFAULT 1;
GO

ALTER TABLE [Curso] ADD [ImagenUrl] nvarchar(max) NULL;
GO

ALTER TABLE [Curso] ADD [PublicId] nvarchar(max) NULL;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260225183350_AgregarEstadoEImagenACurso', N'8.0.23');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [Curso] ADD [ResourceType] nvarchar(20) NULL;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260226211701_AgregarCampoTipoACurso', N'8.0.23');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

DECLARE @var11 sysname;
SELECT @var11 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Calificacion]') AND [c].[name] = N'Comentario');
IF @var11 IS NOT NULL EXEC(N'ALTER TABLE [Calificacion] DROP CONSTRAINT [' + @var11 + '];');
ALTER TABLE [Calificacion] ALTER COLUMN [Comentario] nvarchar(150) NULL;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260228203511_EditarTipoComentarioCalificacion', N'8.0.23');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [AspNetUsers] ADD [FotoUrl] nvarchar(max) NULL DEFAULT N'';
GO

ALTER TABLE [AspNetUsers] ADD [PublicId] nvarchar(max) NULL DEFAULT N'';
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260301123318_AgregarFotoPerfilUsuario', N'8.0.23');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

CREATE TABLE [ActivityLogs] (
    [Id] int NOT NULL IDENTITY,
    [UsuarioId] nvarchar(max) NOT NULL,
    [Accion] nvarchar(max) NOT NULL,
    [Fecha] datetime2 NOT NULL,
    CONSTRAINT [PK_ActivityLogs] PRIMARY KEY ([Id])
);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260301134718_CrearActivityLogs', N'8.0.23');
GO

COMMIT;
GO

