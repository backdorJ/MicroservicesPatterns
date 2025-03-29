
## Запуск

Запустите скрипт для каждого сервиса

### Первый сервис InventoryService - отвечает за проверку наличия товара
```sql
create database inventoryDb;


create table products
(
    id serial primary key,
    product_name text,
    price decimal,
    product_count bigint,
    description varchar
);

create table transactions
(
    id serial primary key,
    unique_transaction_id varchar
);

```

### Второй сервис OrderService - отвечает за оформаление заказа
```sql
create database orderdb;

create table orders(
    id serial primary key,
    user_id int,
    amount decimal,
    product_id int
);

```

### Третий сервис PaymentService - отвечает за оплату заказа

```sql
create database paymentdb;


create table payments(
    id serial primary key,
    user_id int,
    price int,
    product_id int,
)

```

### Четвертый сервис Shipping Service - отвечает за доставку товара

```sql
create table shippingdb;

create table shippings(
    id serial primary key,
    product_id int,
    user_id int
);

```

### Так как мы используем утилиты PostgreSQL то нам нужно включить такую штуку как 
max_prepared_transactions = 100
### Самая важная штука
TransactionId нужно отправлять уникальный всем сервисам, потому что если он будет один, то сервисы не смогут создать его, так как будет дубликат

