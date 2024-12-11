CREATE DATABASE [Cab]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'Cab', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL15.MSSQLSERVER\MSSQL\DATA\Cab.mdf' , SIZE = 8192KB , FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'Cab_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL15.MSSQLSERVER\MSSQL\DATA\Cab_log.ldf' , SIZE = 8192KB , FILEGROWTH = 65536KB )
GO
ALTER DATABASE [Cab] SET COMPATIBILITY_LEVEL = 150
GO
ALTER DATABASE [Cab] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [Cab] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [Cab] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [Cab] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [Cab] SET ARITHABORT OFF 
GO
ALTER DATABASE [Cab] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [Cab] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [Cab] SET AUTO_CREATE_STATISTICS ON(INCREMENTAL = OFF)
GO
ALTER DATABASE [Cab] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [Cab] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [Cab] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [Cab] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [Cab] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [Cab] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [Cab] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [Cab] SET  DISABLE_BROKER 
GO
ALTER DATABASE [Cab] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [Cab] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [Cab] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [Cab] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [Cab] SET  READ_WRITE 
GO
ALTER DATABASE [Cab] SET RECOVERY FULL 
GO
ALTER DATABASE [Cab] SET  MULTI_USER 
GO
ALTER DATABASE [Cab] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [Cab] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [Cab] SET DELAYED_DURABILITY = DISABLED 
GO
USE [Cab]
GO
ALTER DATABASE SCOPED CONFIGURATION SET LEGACY_CARDINALITY_ESTIMATION = Off;
GO
ALTER DATABASE SCOPED CONFIGURATION FOR SECONDARY SET LEGACY_CARDINALITY_ESTIMATION = Primary;
GO
ALTER DATABASE SCOPED CONFIGURATION SET MAXDOP = 0;
GO
ALTER DATABASE SCOPED CONFIGURATION FOR SECONDARY SET MAXDOP = PRIMARY;
GO
ALTER DATABASE SCOPED CONFIGURATION SET PARAMETER_SNIFFING = On;
GO
ALTER DATABASE SCOPED CONFIGURATION FOR SECONDARY SET PARAMETER_SNIFFING = Primary;
GO
ALTER DATABASE SCOPED CONFIGURATION SET QUERY_OPTIMIZER_HOTFIXES = Off;
GO
ALTER DATABASE SCOPED CONFIGURATION FOR SECONDARY SET QUERY_OPTIMIZER_HOTFIXES = Primary;
GO
USE [Cab]
GO
IF NOT EXISTS (SELECT name FROM sys.filegroups WHERE is_default=1 AND name = N'PRIMARY') ALTER DATABASE [Cab] MODIFY FILEGROUP [PRIMARY] DEFAULT
GO
