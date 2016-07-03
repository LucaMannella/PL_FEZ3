-- phpMyAdmin SQL Dump
-- version 4.3.11
-- http://www.phpmyadmin.net
--
-- Host: 127.0.0.1
-- Generation Time: Giu 15, 2016 alle 15:52
-- Versione del server: 5.6.24
-- PHP Version: 5.6.8

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
SET time_zone = "+00:00";

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8 */;

--
-- Database: `pl_fez03`
--
CREATE DATABASE IF NOT EXISTS `pl_fez03` DEFAULT CHARACTER SET utf8 COLLATE utf8_general_ci;
USE `pl_fez03`;
-- --------------------------------------------------------

--
-- Struttura della tabella `clients`
-- Creazione: Mag 29, 2016 alle 15:42
--
DROP TABLE IF EXISTS `clients`;
CREATE TABLE IF NOT EXISTS `clients` (
  `MAC` char(17) NOT NULL,
  `Port` int(10) unsigned NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
-- --------------------------------------------------------

--
-- Struttura della tabella `customers`
-- Creazione: Giu 14, 2016 alle 20:57
--
DROP TABLE IF EXISTS `customers`;
CREATE TABLE IF NOT EXISTS `customers` (
  `MAC` char(17) NOT NULL,
  `email` varchar(100) NOT NULL,
  `pin` varchar(64) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
-- --------------------------------------------------------

--
-- Struttura della tabella `suspicious_pictures`
-- Creazione: Giu 10, 2016 alle 09:46
--
DROP TABLE IF EXISTS `suspicious_pictures`;
CREATE TABLE IF NOT EXISTS `suspicious_pictures` (
  `MAC` char(17) NOT NULL,
  `Timestamp` bigint(20) NOT NULL,
  `Path` varchar(200) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
-- --------------------------------------------------------

--
-- Struttura della tabella `test`
-- Creazione: Mag 20, 2016 alle 12:07
--
DROP TABLE IF EXISTS `test`;
CREATE TABLE IF NOT EXISTS `test` (
  `ID` int(11) NOT NULL,
  `Name` varchar(30) NOT NULL
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8;

--
-- Dump dei dati per la tabella `test`
--
INSERT INTO `test` (`ID`, `Name`) VALUES
(1, 'Alfonso'),
(2, 'Luca');
-- --------------------------------------------------------

--
-- Indexes for table `clients`
--
ALTER TABLE `clients`
  ADD PRIMARY KEY (`MAC`);

--
-- Indexes for table `customers`
--
ALTER TABLE `customers`
  ADD PRIMARY KEY (`MAC`), ADD UNIQUE KEY `email` (`email`);

--
-- Indexes for table `suspicious_pictures`
--
ALTER TABLE `suspicious_pictures`
  ADD PRIMARY KEY (`MAC`,`Timestamp`), ADD UNIQUE KEY `Path` (`Path`);

--
-- Indexes for table `test`
--
ALTER TABLE `test`
  ADD PRIMARY KEY (`ID`);
-- --------------------------------------------------------

--
-- AUTO_INCREMENT for table `test`
--
ALTER TABLE `test`
  MODIFY `ID` int(11) NOT NULL AUTO_INCREMENT,AUTO_INCREMENT=3;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;