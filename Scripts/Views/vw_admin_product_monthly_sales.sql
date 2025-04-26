CREATE VIEW vw_admin_product_monthly_sales AS
SELECT u.id                                   AS admin_id,
       u.user_name                            AS admin_username,
       p.id                                   AS product_id,
       p.name                                 AS product_name,
       s.transaction_date::date               AS transaction_date,
       SUM(s."UnitPrice" * s.quantity)        AS total_value,
       COUNT(*)                               AS transaction_count,
       SUM(s.quantity)                        AS total_items_sold
FROM sale s
         INNER JOIN aspnet_user u ON s.admin_id = u.id
         INNER JOIN product p ON s.product_id = p.id
WHERE s.deleted_at IS NULL
GROUP BY u.id,
         u.user_name,
         p.id,
         p.name,
         s.transaction_date::date
ORDER BY u.user_name,
         p.name,
         transaction_date