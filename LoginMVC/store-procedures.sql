CREATE OR ALTER PROCEDURE dbo.usp_Product_Create
(
    @Name NVARCHAR(50),
    @PurchasePrice DECIMAL(18,2),
    @SalePrice DECIMAL(18,2),
    @Quantity INT,
    @ProductId INT OUTPUT
)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO Products
    (
        Name,
        PurchasePrice,
        SalePrice,
        Quantity
    )
    VALUES
    (
        @Name,
        @PurchasePrice,
        @SalePrice,
        @Quantity
    );

    SET @ProductId = SCOPE_IDENTITY();
END
GO
CREATE OR ALTER PROCEDURE dbo.usp_Product_Update
(
    @ProductId INT,
    @Name NVARCHAR(50),
    @PurchasePrice DECIMAL(18,2),
    @SalePrice DECIMAL(18,2),
    @Quantity INT
)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE Products
    SET
        Name = @Name,
        PurchasePrice = @PurchasePrice,
        SalePrice = @SalePrice,
        Quantity = @Quantity
    WHERE Id = @ProductId;
    SELECT @@ROWCOUNT;
END
GO
CREATE OR ALTER PROCEDURE dbo.usp_Product_Delete
(
    @ProductId INT
)
AS
BEGIN
    SET NOCOUNT ON;

    DELETE Products
    WHERE Id = @ProductId;
    SELECT @@ROWCOUNT;
END
GO
CREATE OR ALTER PROCEDURE dbo.usp_Product_GetById
(
    @ProductId INT
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        Id,
        Name,
        PurchasePrice,
        SalePrice,
        Quantity
    FROM Products
    WHERE Id = @ProductId;
END
GO
CREATE OR ALTER PROCEDURE dbo.usp_Product_Get
(
    @Name NVARCHAR(50) = NULL,
    @OrderBy NVARCHAR(50) = 'id',
    @SortOrder NVARCHAR(4) = 'asc'
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        Id,
        Name,
        PurchasePrice,
        SalePrice,
        Quantity
    FROM Products
    WHERE
        @Name IS NULL
        OR Name LIKE '%' + @Name + '%'

    ORDER BY

        CASE WHEN @OrderBy = 'id'
              AND @SortOrder = 'asc'
             THEN Id END ASC,

        CASE WHEN @OrderBy = 'id'
              AND @SortOrder = 'desc'
             THEN Id END DESC,

        CASE WHEN @OrderBy = 'name'
              AND @SortOrder = 'asc'
             THEN Name END ASC,

        CASE WHEN @OrderBy = 'name'
              AND @SortOrder = 'desc'
             THEN Name END DESC,

        CASE WHEN @OrderBy = 'purchaseprice'
              AND @SortOrder = 'asc'
             THEN PurchasePrice END ASC,

        CASE WHEN @OrderBy = 'purchaseprice'
              AND @SortOrder = 'desc'
             THEN PurchasePrice END DESC,

        CASE WHEN @OrderBy = 'saleprice'
              AND @SortOrder = 'asc'
             THEN SalePrice END ASC,

        CASE WHEN @OrderBy = 'saleprice'
              AND @SortOrder = 'desc'
             THEN SalePrice END DESC,

        CASE WHEN @OrderBy = 'quantity'
              AND @SortOrder = 'asc'
             THEN Quantity END ASC,

        CASE WHEN @OrderBy = 'quantity'
              AND @SortOrder = 'desc'
             THEN Quantity END DESC;
END
GO
CREATE OR ALTER PROCEDURE dbo.usp_Product_GetPaginated
(
    @Name NVARCHAR(50) = NULL,
    @OrderBy NVARCHAR(50) = 'id',
    @SortOrder NVARCHAR(4) = 'asc',
    @PageNumber INT,
    @PageSize INT
)
AS
BEGIN
    SET NOCOUNT ON;

    ;WITH FilteredProducts AS
    (
        SELECT
            Id,
            Name,
            PurchasePrice,
            SalePrice,
            Quantity
        FROM Products
        WHERE
            @Name IS NULL
            OR Name LIKE '%' + @Name + '%'
    )
    SELECT
        Id,
        Name,
        PurchasePrice,
        SalePrice,
        Quantity
    FROM FilteredProducts
    ORDER BY
        CASE WHEN @OrderBy = 'id' AND @SortOrder = 'asc' THEN Id END ASC,
        CASE WHEN @OrderBy = 'id' AND @SortOrder = 'desc' THEN Id END DESC,
        CASE WHEN @OrderBy = 'name' AND @SortOrder = 'asc' THEN Name END ASC,
        CASE WHEN @OrderBy = 'name' AND @SortOrder = 'desc' THEN Name END DESC,
        CASE WHEN @OrderBy = 'purchaseprice' AND @SortOrder = 'asc' THEN PurchasePrice END ASC,
        CASE WHEN @OrderBy = 'purchaseprice' AND @SortOrder = 'desc' THEN PurchasePrice END DESC,
        CASE WHEN @OrderBy = 'saleprice' AND @SortOrder = 'asc' THEN SalePrice END ASC,
        CASE WHEN @OrderBy = 'saleprice' AND @SortOrder = 'desc' THEN SalePrice END DESC,
        CASE WHEN @OrderBy = 'quantity' AND @SortOrder = 'asc' THEN Quantity END ASC,
        CASE WHEN @OrderBy = 'quantity' AND @SortOrder = 'desc' THEN Quantity END DESC
    OFFSET (@PageNumber - 1) * @PageSize ROWS
    FETCH NEXT @PageSize ROWS ONLY;

    SELECT COUNT(*)
    FROM Products
    WHERE
        @Name IS NULL
        OR Name LIKE '%' + @Name + '%';
END
GO