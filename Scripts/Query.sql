DROP DATABASE IF EXISTS PruebaUnilink
CREATE DATABASE PruebaUnilink

DROP TABLE IF EXISTS Usuarios
CREATE TABLE Usuarios (

	Nombre VARCHAR(100) NOT NULL
   ,Saldo DECIMAL(18, 2) NOT NULL
)

CREATE OR ALTER PROCEDURE dbo.spObtenerUsuario (@nombre VARCHAR(100))
AS
	SELECT TOP 1
		u.Nombre
	   ,u.Saldo
	FROM Usuarios u
	WHERE LTRIM(RTRIM(u.Nombre)) = @nombre
GO

CREATE OR ALTER PROCEDURE dbo.spCargarSaldo (@nombre VARCHAR(100))
AS
	SELECT TOP 1
		u.Saldo
	FROM Usuarios u
	WHERE LTRIM(RTRIM(u.Nombre)) = @nombre
GO

CREATE OR ALTER PROCEDURE dbo.spGuardarDatos (@nombre VARCHAR(100), @saldo DECIMAL(18, 2))
AS
BEGIN
	SET NOCOUNT ON;

	IF EXISTS (SELECT
				1
			FROM Usuarios
			WHERE LTRIM(RTRIM(Nombre)) = @nombre)
	BEGIN
		-- El usuario existe, realizar UPDATE
		UPDATE Usuarios
		SET Saldo = @saldo
		WHERE LTRIM(RTRIM(Nombre)) = @nombre;

		IF @@rowcount > 0
		BEGIN
			SELECT
				u.Nombre
			   ,u.Saldo
			FROM Usuarios u
			WHERE LTRIM(RTRIM(u.Nombre)) = @nombre;

			SELECT
				1 AS GuardadoExitoso;
		END
		ELSE
		BEGIN
			SELECT
				0 AS GuardadoExitoso;
		END
	END
	ELSE
	BEGIN
		-- El usuario no existe, realizar INSERT
		INSERT INTO Usuarios (Nombre, Saldo)
			VALUES (@nombre, @saldo);

		IF @@rowcount > 0
		BEGIN
			SELECT
				u.Nombre
			   ,u.Saldo
			FROM Usuarios u
			WHERE LTRIM(RTRIM(u.Nombre)) = @nombre;

			SELECT
				1 AS GuardadoExitoso;
		END
		ELSE
		BEGIN
			SELECT
				0 AS GuardadoExitoso;
		END
	END
END
GO