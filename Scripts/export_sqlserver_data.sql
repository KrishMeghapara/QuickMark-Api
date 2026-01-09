-- =============================================
-- SQL Server Data Export Script for QuickMart
-- Run this in SQL Server Management Studio (SSMS)
-- =============================================

-- This script will generate INSERT statements for all your data
-- You can then run these in PostgreSQL (with minor modifications)

-- First, let's see what tables exist and their row counts
SELECT 
    t.name AS TableName,
    p.rows AS RowCount
FROM sys.tables t
INNER JOIN sys.partitions p ON t.object_id = p.object_id
WHERE p.index_id IN (0, 1)
ORDER BY t.name;

-- To export data, use one of these methods:

-- METHOD 1: Use SSMS "Generate Scripts" Wizard
-- 1. Right-click on your database in SSMS
-- 2. Tasks → Generate Scripts
-- 3. Select "Specific database objects" → Tables
-- 4. Click Next → Advanced
-- 5. Set "Types of data to script" = "Data only"
-- 6. Save to file

-- METHOD 2: Use BCP (Bulk Copy Program) from Command Line
-- Run these commands in Command Prompt (replace YOUR_SERVER and YOUR_DATABASE):

/*
bcp "SELECT * FROM [Category]" queryout "D:\asp proj\export\Category.csv" -c -t"," -S YOUR_SERVER -d YOUR_DATABASE -T
bcp "SELECT * FROM [User]" queryout "D:\asp proj\export\User.csv" -c -t"," -S YOUR_SERVER -d YOUR_DATABASE -T
bcp "SELECT * FROM [Address]" queryout "D:\asp proj\export\Address.csv" -c -t"," -S YOUR_SERVER -d YOUR_DATABASE -T
bcp "SELECT * FROM [Product]" queryout "D:\asp proj\export\Product.csv" -c -t"," -S YOUR_SERVER -d YOUR_DATABASE -T
bcp "SELECT * FROM [Orders]" queryout "D:\asp proj\export\Orders.csv" -c -t"," -S YOUR_SERVER -d YOUR_DATABASE -T
bcp "SELECT * FROM [OrderItem]" queryout "D:\asp proj\export\OrderItem.csv" -c -t"," -S YOUR_SERVER -d YOUR_DATABASE -T
bcp "SELECT * FROM [Cart]" queryout "D:\asp proj\export\Cart.csv" -c -t"," -S YOUR_SERVER -d YOUR_DATABASE -T
bcp "SELECT * FROM [ProductReview]" queryout "D:\asp proj\export\ProductReview.csv" -c -t"," -S YOUR_SERVER -d YOUR_DATABASE -T
*/
