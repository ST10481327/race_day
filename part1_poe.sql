-- =============================================
-- RaceDay System — Database Script
-- Script Name: part1_poe
-- Creates schema + seed data
-- =============================================

-- Safely drop existing database by severing active connections
IF EXISTS (SELECT name FROM sys.databases WHERE name = N'part1_poe')
BEGIN
    ALTER DATABASE part1_poe SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE part1_poe;
END;

-- Create fresh database
CREATE DATABASE part1_poe;

-- Explicitly switch context to the new database
USE part1_poe;

-- =============================================
-- 1. Roles Table
-- =============================================
CREATE TABLE Role (
    role_id INT IDENTITY(1,1) PRIMARY KEY,
    role_name VARCHAR(50) NOT NULL UNIQUE,
    description VARCHAR(200)
);

-- =============================================
-- 2. Users Table
-- =============================================
CREATE TABLE [User] (
    user_id INT IDENTITY(1,1) PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    email VARCHAR(100) NOT NULL UNIQUE,
    password_hash VARCHAR(255) NOT NULL,
    phone VARCHAR(20),
    address VARCHAR(255),
    created_at DATETIME DEFAULT GETDATE(),
    role_id INT NOT NULL,
    FOREIGN KEY (role_id) REFERENCES Role(role_id)
);

-- =============================================
-- 3. Event Categories Table
-- =============================================
CREATE TABLE EventCategory (
    category_id INT IDENTITY(1,1) PRIMARY KEY,
    category_name VARCHAR(100) NOT NULL,
    description VARCHAR(255),
    distance_km DECIMAL(5,2) NOT NULL,
    CONSTRAINT UQ_CategoryName UNIQUE (category_name)
);

-- =============================================
-- 4. Events Table
-- =============================================
CREATE TABLE [Event] (
    event_id INT IDENTITY(1,1) PRIMARY KEY,
    event_name VARCHAR(150) NOT NULL,
    description TEXT,
    event_date DATE NOT NULL,
    location VARCHAR(150) NOT NULL,
    max_participants INT NOT NULL CHECK (max_participants > 0),
    status VARCHAR(20) DEFAULT 'Upcoming' CHECK (status IN ('Upcoming', 'Open', 'Closed', 'Completed', 'Cancelled')),
    organiser_id INT NOT NULL,
    category_id INT NOT NULL,
    FOREIGN KEY (organiser_id) REFERENCES [User](user_id),
    FOREIGN KEY (category_id) REFERENCES EventCategory(category_id)
);

-- =============================================
-- 5. Event Enrolments Table
-- =============================================
CREATE TABLE EventEnrolment (
    enrolment_id INT IDENTITY(1,1) PRIMARY KEY,
    enrolment_date DATE DEFAULT CAST(GETDATE() AS DATE),
    status VARCHAR(20) DEFAULT 'Pending' CHECK (status IN ('Pending', 'Approved', 'Rejected', 'Withdrawn')),
    user_id INT NOT NULL,
    event_id INT NOT NULL,
    FOREIGN KEY (user_id) REFERENCES [User](user_id) ON DELETE CASCADE,
    FOREIGN KEY (event_id) REFERENCES [Event](event_id) ON DELETE CASCADE,
    CONSTRAINT UQ_UserEvent UNIQUE (user_id, event_id)
);

-- =============================================
-- 6. Results Table
-- =============================================
CREATE TABLE Result (
    result_id INT IDENTITY(1,1) PRIMARY KEY,
    finish_time TIME NOT NULL,
    position INT CHECK (position > 0),
    notes VARCHAR(255),
    enrolment_id INT NOT NULL UNIQUE,
    FOREIGN KEY (enrolment_id) REFERENCES EventEnrolment(enrolment_id) ON DELETE CASCADE
);

-- =============================================
-- SEED DATA
-- =============================================

-- Roles
INSERT INTO Role (role_name, description) VALUES
('Admin', 'System administrator'),
('Organiser', 'Can create and manage events'),
('Participant', 'Can enrol and view results');

-- Users
INSERT INTO [User] (name, email, password_hash, phone, address, role_id) VALUES
('Sarah Johnson', 'sarah@racehub.com', 'HASH_ORG1', '011-555-0101', 'Johannesburg', 2),
('Mike Dube', 'mike@racehub.com', 'HASH_ORG2', '011-555-0102', 'Pretoria', 2),
('Thabo Mkhize', 'thabo@email.com', 'HASH_P1', '082-555-0103', 'Sandton', 3),
('Lerato Ndlovu', 'lerato@email.com', 'HASH_P2', '083-555-0104', 'Midrand', 3);

-- Categories
INSERT INTO EventCategory (category_name, description, distance_km) VALUES
('5K Fun Run', 'Short distance recreational run', 5.00),
('10K Road Race', 'Standard 10km road event', 10.00),
('Half Marathon', '21.1km intermediate distance', 21.10),
('Full Marathon', '42.2km classic endurance race', 42.20);

-- Events
INSERT INTO [Event] (event_name, description, event_date, location, max_participants, status, organiser_id, category_id) VALUES
('Johannesburg Spring Run', 'Annual spring 10km through the city', '2026-10-15', 'Johannesburg Zoo', 500, 'Upcoming', 1, 2),
('Sandton 5K Twilight', 'Evening fun run with lights', '2026-11-05', 'Sandton City', 300, 'Open', 1, 1),
('Pretoria Half Marathon', 'Scenic route around the Union Buildings', '2026-12-01', 'Pretoria CBD', 800, 'Open', 2, 3);

-- Enrolments
INSERT INTO EventEnrolment (status, user_id, event_id) VALUES
('Approved', 3, 1),
('Pending', 4, 1),
('Approved', 3, 2),
('Approved', 4, 3);

-- Results
INSERT INTO Result (finish_time, position, notes, enrolment_id) VALUES
('00:48:32', 12, 'Personal best!', 1),
('00:35:20', 8, 'Great run!', 3);

-- =============================================
-- VERIFICATION
-- =============================================
SELECT 'Role' AS TableName, COUNT(*) AS TotalRecords FROM Role 
UNION ALL SELECT 'User', COUNT(*) FROM [User] 
UNION ALL SELECT 'EventCategory', COUNT(*) FROM EventCategory 
UNION ALL SELECT 'Event', COUNT(*) FROM [Event] 
UNION ALL SELECT 'EventEnrolment', COUNT(*) FROM EventEnrolment 
UNION ALL SELECT 'Result', COUNT(*) FROM Result;

