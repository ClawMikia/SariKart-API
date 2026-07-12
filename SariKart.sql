/*
 =============================================================================
    SariKart API - Database Schema (SQL Server)
    Generated for: KiNoDBFightingMachine (Somee)
    Run this entire script once against the empty database to create all
    tables, foreign keys, indexes and seed the required lookup data.
 =============================================================================
*/

/* ---------------------------------------------------------------------------
   Lookup / reference tables
--------------------------------------------------------------------------- */
CREATE TABLE [UserType] (
    [Id]       INT           IDENTITY(1,1) PRIMARY KEY,
    [UserType] NVARCHAR(50)  NOT NULL
);
GO

CREATE TABLE [OrderStatus] (
    [Id]          INT           IDENTITY(1,1) PRIMARY KEY,
    [OrderStatus] NVARCHAR(50)  NOT NULL
);
GO

CREATE TABLE [Vehicle] (
    [Id]   INT           IDENTITY(1,1) PRIMARY KEY,
    [Type] NVARCHAR(50)  NOT NULL
);
GO

CREATE TABLE [Category] (
    [Id]       INT           IDENTITY(1,1) PRIMARY KEY,
    [Category] NVARCHAR(50)  NOT NULL,
    [Icon]     NVARCHAR(50)  NOT NULL
);
GO

CREATE TABLE [StoreBranch] (
    [Id]       INT           IDENTITY(1,1) PRIMARY KEY,
    [Branch]   NVARCHAR(50)  NOT NULL,
    [Street]   NVARCHAR(50)  NOT NULL,
    [City]     NVARCHAR(50)  NOT NULL,
    [Province] NVARCHAR(50)  NOT NULL
);
GO

/* ---------------------------------------------------------------------------
   Users
--------------------------------------------------------------------------- */
CREATE TABLE [AppUser] (
    [Id]                     INT           IDENTITY(1,1) PRIMARY KEY,
    [FirstName]              NVARCHAR(50)  NOT NULL,
    [LastName]               NVARCHAR(50)  NOT NULL,
    [Username]               NVARCHAR(50)  NOT NULL,
    [Password]               NVARCHAR(50)  NOT NULL,
    [ContactNumber]          NVARCHAR(50)  NOT NULL,
    [CreatedAt]              DATETIME      NOT NULL DEFAULT (GETDATE()),
    [UserTypeId]             INT           NOT NULL,
    [EditableContactPerson]  NVARCHAR(50)  NULL,
    [EditableContactNumber]  NVARCHAR(50)  NULL,
    [EditableContactAddress] NVARCHAR(MAX) NULL,
    CONSTRAINT [FK_AppUser_UserType] FOREIGN KEY ([UserTypeId])
        REFERENCES [UserType]([Id])
);
GO

/* ---------------------------------------------------------------------------
   Products
--------------------------------------------------------------------------- */
CREATE TABLE [Product] (
    [Id]         INT           IDENTITY(1,1) PRIMARY KEY,
    [Product]    NVARCHAR(50)  NOT NULL,
    [CategoryId] INT           NOT NULL,
    [Price]      DECIMAL(6,2)  NOT NULL,
    [Unit]       NVARCHAR(50)  NOT NULL,
    [Stock]      INT           NOT NULL,
    [Picture]    NVARCHAR(50)  NOT NULL,
    CONSTRAINT [FK_Product_Category] FOREIGN KEY ([CategoryId])
        REFERENCES [Category]([Id])
);
GO

/* ---------------------------------------------------------------------------
   Orders
--------------------------------------------------------------------------- */
CREATE TABLE [ShopOrder] (
    [Id]            INT           IDENTITY(1,1) PRIMARY KEY,
    [UserId]        INT           NOT NULL,
    [CreateDate]    DATETIME      NOT NULL DEFAULT (GETDATE()),
    [TotalAmount]   DECIMAL(6,2)  NOT NULL,
    [AmountPaid]    DECIMAL(6,2)  NOT NULL,
    [Change]        DECIMAL(6,2)  NOT NULL,
    [OrderStatusId] INT           NOT NULL DEFAULT (1),
    [ContactPerson] NVARCHAR(50)  NOT NULL,
    [ContactNumber] NVARCHAR(50)  NOT NULL,
    [ContactAddress] NVARCHAR(MAX) NOT NULL,
    CONSTRAINT [FK_ShopOrder_OrderStatus] FOREIGN KEY ([OrderStatusId])
        REFERENCES [OrderStatus]([Id]),
    CONSTRAINT [FK_Order_User] FOREIGN KEY ([UserId])
        REFERENCES [AppUser]([Id])
);
GO

CREATE TABLE [UserStore] (
    [Id]      INT           IDENTITY(1,1) PRIMARY KEY,
    [UserId]  INT           NOT NULL,
    [Address] NVARCHAR(50)  NOT NULL,
    CONSTRAINT [FK_UserStore_User] FOREIGN KEY ([UserId])
        REFERENCES [AppUser]([Id])
);
GO

CREATE TABLE [OrderLine] (
    [OrderLineId] INT IDENTITY(1,1) PRIMARY KEY,
    [OrderId]     INT NOT NULL,
    [ProductId]   INT NOT NULL,
    [Quantity]    INT NOT NULL,
    CONSTRAINT [FK_OrderLine_Order] FOREIGN KEY ([OrderId])
        REFERENCES [ShopOrder]([Id]),
    CONSTRAINT [FK_OrderLine_Product] FOREIGN KEY ([ProductId])
        REFERENCES [Product]([Id])
);
GO

CREATE TABLE [Delivery] (
    [Id]          INT      IDENTITY(1,1) PRIMARY KEY,
    [StoreId]     INT      NOT NULL,
    [OrderId]     INT      NOT NULL,
    [RiderId]     INT      NOT NULL,
    [DeliveryDate] DATETIME NOT NULL DEFAULT (GETDATE()),
    [Delivered]   BIT      NOT NULL,
    [VehicleId]   INT      NOT NULL,
    [CashOnHand]  BIT      NOT NULL,
    CONSTRAINT [FK_Delivery_Order]     FOREIGN KEY ([OrderId])  REFERENCES [ShopOrder]([Id]),
    CONSTRAINT [FK_Delivery_AppUser]   FOREIGN KEY ([RiderId])  REFERENCES [AppUser]([Id]),
    CONSTRAINT [FK_Delivery_UserStore] FOREIGN KEY ([StoreId])  REFERENCES [StoreBranch]([Id]),
    CONSTRAINT [FK_Delivery_Vehicle]   FOREIGN KEY ([VehicleId]) REFERENCES [Vehicle]([Id])
);
GO

/* ---------------------------------------------------------------------------
   Indexes (performance)
--------------------------------------------------------------------------- */
CREATE INDEX [IX_AppUser_Username]   ON [AppUser]([Username]);
CREATE INDEX [IX_AppUser_UserTypeId] ON [AppUser]([UserTypeId]);
CREATE INDEX [IX_Product_CategoryId] ON [Product]([CategoryId]);
CREATE INDEX [IX_Product_Product]    ON [Product]([Product]);
CREATE INDEX [IX_ShopOrder_UserId]        ON [ShopOrder]([UserId]);
CREATE INDEX [IX_ShopOrder_OrderStatusId] ON [ShopOrder]([OrderStatusId]);
CREATE INDEX [IX_ShopOrder_CreateDate]    ON [ShopOrder]([CreateDate]);
CREATE INDEX [IX_OrderLine_OrderId]   ON [OrderLine]([OrderId]);
CREATE INDEX [IX_OrderLine_ProductId] ON [OrderLine]([ProductId]);
CREATE INDEX [IX_Delivery_OrderId]    ON [Delivery]([OrderId]);
CREATE INDEX [IX_Delivery_StoreId]    ON [Delivery]([StoreId]);
CREATE INDEX [IX_Delivery_RiderId]    ON [Delivery]([RiderId]);
GO

/* ---------------------------------------------------------------------------
   Seed data (lookups the application depends on)
--------------------------------------------------------------------------- */
SET IDENTITY_INSERT [UserType] ON;
INSERT INTO [UserType] ([Id], [UserType]) VALUES
    (1,    'Customer'),
    (2,    'Admin'),
    (3,    'Rider'),
    (1003, 'Cashier');
SET IDENTITY_INSERT [UserType] OFF;

SET IDENTITY_INSERT [OrderStatus] ON;
INSERT INTO [OrderStatus] ([Id], [OrderStatus]) VALUES
    (1, 'Pending'),
    (2, 'Processing'),
    (3, 'Cancelled'),
    (4, 'Followed Up');
SET IDENTITY_INSERT [OrderStatus] OFF;

SET IDENTITY_INSERT [Vehicle] ON;
INSERT INTO [Vehicle] ([Id], [Type]) VALUES
    (1, 'Motorcycle');
SET IDENTITY_INSERT [Vehicle] OFF;

SET IDENTITY_INSERT [StoreBranch] ON;
INSERT INTO [StoreBranch] ([Id], [Branch], [Street], [City], [Province]) VALUES
    (1, 'Main Branch', 'N/A', 'N/A', 'N/A');
SET IDENTITY_INSERT [StoreBranch] OFF;

-- Default rider (RiderId = 1003) referenced by PlaceOrder, plus demo admin & cashier logins.
SET IDENTITY_INSERT [AppUser] ON;
INSERT INTO [AppUser] ([Id], [FirstName], [LastName], [Username], [Password], [ContactNumber], [UserTypeId]) VALUES
    (2,    'Admin',   'User',    'admin',   'admin',   '09123456789', 2),
    (1003, 'Default', 'Rider',   'rider',   'rider',   'N/A',         3),
    (1004, 'Demo',    'Cashier', 'cashier', 'cashier', 'N/A',         1003);
SET IDENTITY_INSERT [AppUser] OFF;

/* ---------------------------------------------------------------------------
   Demo master data (categories + products) so the app is not empty on first run.
   Safe to delete after loading real data.
--------------------------------------------------------------------------- */
SET IDENTITY_INSERT [Category] ON;
INSERT INTO [Category] ([Id], [Category], [Icon]) VALUES
    (1, 'Rice & Grains', 'rice.png'),
    (2, 'Canned Goods',  'canned.png'),
    (3, 'Snacks',        'snacks.png'),
    (4, 'Beverages',     'beverages.png'),
    (5, 'Vegetables',    'veggies.png'),
    (6, 'Condiments',    'condiments.png');
SET IDENTITY_INSERT [Category] OFF;

SET IDENTITY_INSERT [Product] ON;
INSERT INTO [Product] ([Id], [Product], [CategoryId], [Price], [Unit], [Stock], [Picture]) VALUES
    (1,  'Premium Rice 1kg',      1, 52.00, 'pack',   100, 'rice1.png'),
    (2,  'Brown Rice 1kg',        1, 60.00, 'pack',    80, 'rice2.png'),
    (3,  'Sardines 155g',         2, 25.00, 'can',    150, 'sardines.png'),
    (4,  'Corned Beef 150g',      2, 38.00, 'can',    120, 'cornedbeef.png'),
    (5,  'Chips 50g',             3, 15.00, 'pack',   200, 'chips.png'),
    (6,  'Biscuits',              3, 12.00, 'pack',   180, 'biscuits.png'),
    (7,  'Soda 330ml',            4, 20.00, 'bottle', 300, 'soda.png'),
    (8,  'Mineral Water 500ml',   4, 15.00, 'bottle', 250, 'water.png'),
    (9,  'Tomato',                5, 30.00, 'kg',      60, 'tomato.png'),
    (10, 'Onion',                 5, 40.00, 'kg',      70, 'onion.png'),
    (11, 'Salt 500g',             6, 10.00, 'pack',   100, 'salt.png'),
    (12, 'Soy Sauce 350ml',       6, 22.00, 'bottle',  90, 'soy.png');
SET IDENTITY_INSERT [Product] OFF;

/* ---------------------------------------------------------------------------
   Reseed identity columns so future inserts continue after the seeded ids
--------------------------------------------------------------------------- */
DECLARE @max INT;
SELECT @max = ISNULL(MAX([Id]), 1) FROM [UserType];     DBCC CHECKIDENT ('[UserType]', RESEED, @max);
SELECT @max = ISNULL(MAX([Id]), 1) FROM [OrderStatus];  DBCC CHECKIDENT ('[OrderStatus]', RESEED, @max);
SELECT @max = ISNULL(MAX([Id]), 1) FROM [Vehicle];      DBCC CHECKIDENT ('[Vehicle]', RESEED, @max);
SELECT @max = ISNULL(MAX([Id]), 1) FROM [StoreBranch];  DBCC CHECKIDENT ('[StoreBranch]', RESEED, @max);
SELECT @max = ISNULL(MAX([Id]), 1) FROM [AppUser];      DBCC CHECKIDENT ('[AppUser]', RESEED, @max);
SELECT @max = ISNULL(MAX([Id]), 1) FROM [Category];     DBCC CHECKIDENT ('[Category]', RESEED, @max);
SELECT @max = ISNULL(MAX([Id]), 1) FROM [Product];      DBCC CHECKIDENT ('[Product]', RESEED, @max);
GO
