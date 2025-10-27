#Database for testing using Cassandra

CREATE KEYSPACE IF NOT EXISTS warranty_app_v3
WITH replication = {'class':'SimpleStrategy','replication_factor':1};

USE warranty_app_v3;	

CREATE TABLE users_by_username (
    username TEXT PRIMARY KEY,
    password TEXT,
    full_name TEXT,
    role TEXT,
    status TEXT
);

CREATE TABLE IF NOT EXISTS products_by_serial (
    serial_number TEXT PRIMARY KEY,
    product_name TEXT,
    purchase_date DATE,
    warranty_months INT,
    customer_id TEXT,
    status TEXT,
    image_url TEXT
);

CREATE TABLE IF NOT EXISTS warranty_tickets_by_product (
    ticket_id TEXT,
    serial_number TEXT,
    customer_id TEXT,
    created_at TIMESTAMP,
    issue_description TEXT,
    status TEXT,
    technician_id TEXT,
    PRIMARY KEY (serial_number, ticket_id)
) WITH CLUSTERING ORDER BY (ticket_id DESC);


CREATE TABLE IF NOT EXISTS warranty_tickets_by_technician (
    technician_id TEXT,
    ticket_id TEXT,
    serial_number TEXT,
    customer_id TEXT,
    created_at TIMESTAMP,
    issue_description TEXT,
    status TEXT,
    PRIMARY KEY (technician_id, ticket_id)
) WITH CLUSTERING ORDER BY (ticket_id DESC);

CREATE TABLE IF NOT EXISTS ticket_sequence (
    key TEXT PRIMARY KEY,
    last_number INT
);

CREATE TABLE IF NOT EXISTS customer_sequence (
    key TEXT PRIMARY KEY,
    last_number INT
);

CREATE TABLE IF NOT EXISTS repair_sequence (
    key TEXT PRIMARY KEY,
    last_number INT
);

INSERT INTO repair_sequence (key, last_number) VALUES ('REPAIR', 0);


CREATE TABLE IF NOT EXISTS customers_by_id (
    customer_id TEXT PRIMARY KEY,
    full_name TEXT,
    phone TEXT,
    email TEXT,
    address TEXT,
    status TEXT /* active | deactive */
);


CREATE TABLE IF NOT EXISTS customers_by_phone (
    phone TEXT PRIMARY KEY,
    customer_id TEXT,
    full_name TEXT,
    email TEXT,
    address TEXT,
    status TEXT
);


CREATE TABLE IF NOT EXISTS repairs_by_technician (
    technician_id TEXT,
    repair_id TEXT,
    ticket_id TEXT,
    serial_number TEXT,
    start_date TIMESTAMP,
    complete_date TIMESTAMP,
    repair_description TEXT,
    parts_used TEXT,
    cost DECIMAL,
    status TEXT,  -- pending | in_progress | completed
    PRIMARY KEY (technician_id, repair_id)
) WITH CLUSTERING ORDER BY (repair_id DESC);

USE warranty_app_v3;

INSERT INTO ticket_sequence (key, last_number) VALUES ('TICKET', 0);

INSERT INTO customer_sequence (key, last_number) VALUES ('CUSTOMER', 0);

INSERT INTO repair_sequence (key, last_number) VALUES ('REPAIR', 0);

INSERT INTO users_by_username (username, password, full_name, role, status)
VALUES ('admin', '123', 'Quản trị viên', 'admin', 'active');

INSERT INTO users_by_username (username, password, full_name, role, status)
VALUES ('user01', '123', 'Người dùng 1', 'staff', 'active');


INSERT INTO users_by_username (username, password, full_name, role, status)
VALUES ('admin01', '123', 'Nguyễn Văn Admin', 'admin', 'active');

INSERT INTO users_by_username (username, password, full_name, role, status)
VALUES ('staff01', '123', 'Trần Thị Staff', 'staff', 'active');

INSERT INTO users_by_username (username, password, full_name, role, status)
VALUES ('tech01', '123', 'Lê Văn Tech', 'tech', 'active');

INSERT INTO users_by_username (username, password, full_name, role, status)
VALUES ('tech02', '123', 'Phạm Công Nghệ', 'tech', 'active');

INSERT INTO users_by_username (username, password, full_name, role, status)
VALUES ('tech03', '123', 'Trương Kỹ Thuật', 'tech', 'inactive');


INSERT INTO products_by_serial (serial_number, product_name, purchase_date, warranty_months, customer_id, status, image_url)
VALUES ('SP0001', 'iPhone 15 Pro Max', '2024-03-10', 24, 'CUST001', 'active', 'https://images.unsplash.com/photo-1695048138154-d16243f6d2ba');

INSERT INTO products_by_serial (serial_number, product_name, purchase_date, warranty_months, customer_id, status, image_url)
VALUES ('SP0002', 'iPhone 14 Pro', '2023-07-15', 24, 'CUST002', 'active', 'https://images.unsplash.com/photo-1675865396531-12a1ed7e9a60');

INSERT INTO products_by_serial (serial_number, product_name, purchase_date, warranty_months, customer_id, status, image_url)
VALUES ('SP0003', 'Samsung Galaxy S23 Ultra', '2023-11-01', 12, 'CUST003', 'active', 'https://images.unsplash.com/photo-1682687218982-6c3f4ace0ebf');

INSERT INTO products_by_serial (serial_number, product_name, purchase_date, warranty_months, customer_id, status, image_url)
VALUES ('SP0004', 'Samsung Galaxy Z Fold 5', '2024-01-20', 36, 'CUST004', 'active', 'https://images.unsplash.com/photo-1699025954973-281e282ecfa9');

INSERT INTO products_by_serial (serial_number, product_name, purchase_date, warranty_months, customer_id, status, image_url)
VALUES ('SP0005', 'Xiaomi Mi 13', '2024-02-01', 24, 'CUST005', 'active', 'https://images.unsplash.com/photo-1606813902777-0ad5d7f4d6a0');

INSERT INTO products_by_serial (serial_number, product_name, purchase_date, warranty_months, customer_id, status, image_url)
VALUES ('SP0006', 'Xiaomi Redmi Note 12', '2023-12-01', 12, 'CUST006', 'active', 'https://images.unsplash.com/photo-1603170336272-e0d54b0a97a9');

INSERT INTO products_by_serial (serial_number, product_name, purchase_date, warranty_months, customer_id, status, image_url)
VALUES ('SP0007', 'iPhone 13', '2023-09-10', 24, 'CUST007', 'active', 'https://images.unsplash.com/photo-1618077360398-4d5b9b118db3');


INSERT INTO products_by_serial (serial_number, product_name, purchase_date, warranty_months, customer_id, status, image_url)
VALUES ('SP0008', 'iPhone 12', '2021-05-01', 12, 'CUST008', 'active', 'https://images.unsplash.com/photo-1580910051074-9c6f9fe7f1a0');

INSERT INTO products_by_serial (serial_number, product_name, purchase_date, warranty_months, customer_id, status, image_url)
VALUES ('SP0009', 'Samsung Galaxy S21', '2021-01-15', 24, 'CUST009', 'active', 'https://images.unsplash.com/photo-1616701093981-aa6bb49a58ad');

INSERT INTO products_by_serial (serial_number, product_name, purchase_date, warranty_months, customer_id, status, image_url)
VALUES ('SP0010', 'Xiaomi Mi 11', '2020-12-10', 24, 'CUST010', 'active', 'https://images.unsplash.com/photo-1612201898125-1ec154e32a07');



INSERT INTO products_by_serial (serial_number, product_name, purchase_date, warranty_months, customer_id, status, image_url)
VALUES ('SP0011', 'MacBook Pro 14', '2024-01-10', 24, 'CUST011', 'active', 'https://images.unsplash.com/photo-1610465299995-6b54598f1a76');

INSERT INTO products_by_serial (serial_number, product_name, purchase_date, warranty_months, customer_id, status, image_url)
VALUES ('SP0012', 'Dell XPS 15', '2023-11-01', 24, 'CUST012', 'active', 'https://images.unsplash.com/photo-1502877338535-766e1452684a');

INSERT INTO products_by_serial (serial_number, product_name, purchase_date, warranty_months, customer_id, status, image_url)
VALUES ('SP0013', 'HP EliteBook', '2024-03-01', 12, 'CUST013', 'active', 'https://images.unsplash.com/photo-1587831990711-23ca6441447b');

INSERT INTO products_by_serial (serial_number, product_name, purchase_date, warranty_months, customer_id, status, image_url)
VALUES ('SP0014', 'MacBook Air M2', '2023-09-05', 24, 'CUST014', 'active', 'https://images.unsplash.com/photo-1629425733761-c21c9f074d82');

INSERT INTO products_by_serial (serial_number, product_name, purchase_date, warranty_months, customer_id, status, image_url)
VALUES ('SP0015', 'Dell Inspiron 5515', '2024-02-15', 12, 'CUST015', 'active', 'https://images.unsplash.com/photo-1498050108023-c5249f4df085');

INSERT INTO products_by_serial (serial_number, product_name, purchase_date, warranty_months, customer_id, status, image_url)
VALUES ('SP0016', 'HP Spectre x360', '2023-10-01', 24, 'CUST016', 'active', 'https://images.unsplash.com/photo-1587824745339-27fdb78f06c2');

INSERT INTO products_by_serial (serial_number, product_name, purchase_date, warranty_months, customer_id, status, image_url)
VALUES ('SP0017', 'Lenovo ThinkPad X1', '2024-03-20', 36, 'CUST017', 'active', 'https://images.unsplash.com/photo-1611078483040-f8a8b2ddb3f2');



INSERT INTO products_by_serial (serial_number, product_name, purchase_date, warranty_months, customer_id, status, image_url)
VALUES ('SP0018', 'MacBook Pro 2019', '2020-01-10', 24, 'CUST018', 'active', 'https://images.unsplash.com/photo-1481277542470-605612bd2d61');

INSERT INTO products_by_serial (serial_number, product_name, purchase_date, warranty_months, customer_id, status, image_url)
VALUES ('SP0019', 'Dell Latitude 5500', '2021-05-18', 12, 'CUST019', 'active', 'https://images.unsplash.com/photo-1496181133206-80ce9b88a853');

INSERT INTO products_by_serial (serial_number, product_name, purchase_date, warranty_months, customer_id, status, image_url)
VALUES ('SP0020', 'HP Pavilion 15', '2021-03-01', 12, 'CUST020', 'active', 'https://images.unsplash.com/photo-1622464219605-7a6b690a5b3d');


INSERT INTO products_by_serial (serial_number, product_name, purchase_date, warranty_months, customer_id, status, image_url) VALUES ('SP0021','Samsung QLED 4K','2024-01-15',24,'CUST001','active','https://images.unsplash.com/photo-1593359677878-68fca36a3765');
INSERT INTO products_by_serial (serial_number, product_name, purchase_date, warranty_months, customer_id, status, image_url) VALUES ('SP0022','LG OLED 55 inch','2023-06-01',36,'CUST002','active','https://images.unsplash.com/photo-1606813902777-0ad5d7f4d6a0');
INSERT INTO products_by_serial (serial_number, product_name, purchase_date, warranty_months, customer_id, status, image_url) VALUES ('SP0023','Sony Bravia 4K','2023-10-10',24,'CUST003','active','https://images.unsplash.com/photo-1556905055-8f358a7a47b2');
INSERT INTO products_by_serial (serial_number, product_name, purchase_date, warranty_months, customer_id, status, image_url) VALUES ('SP0024','Samsung Crystal UHD','2024-02-22',12,'CUST004','active','https://images.unsplash.com/photo-1587825140400-bd0fc7aca3f9');
INSERT INTO products_by_serial (serial_number, product_name, purchase_date, warranty_months, customer_id, status, image_url) VALUES ('SP0025','LG NanoCell TV','2023-12-05',24,'CUST005','active','https://images.unsplash.com/photo-1605887443498-4f6aeb5b088e');
INSERT INTO products_by_serial (serial_number, product_name, purchase_date, warranty_months, customer_id, status, image_url) VALUES ('SP0026','Sony XR OLED','2024-03-01',24,'CUST006','active','https://images.unsplash.com/photo-1606813853103-6d5c2c064b41');
INSERT INTO products_by_serial (serial_number, product_name, purchase_date, warranty_months, customer_id, status, image_url) VALUES ('SP0027','Samsung Frame TV','2023-11-11',24,'CUST007','active','https://images.unsplash.com/photo-1593643946898-62ca1d1e35a7');
INSERT INTO products_by_serial (serial_number, product_name, purchase_date, warranty_months, customer_id, status, image_url) VALUES ('SP0028','Sony Bravia 2019','2020-05-01',12,'CUST008','active','https://images.unsplash.com/photo-1481277542470-605612bd2d61');
INSERT INTO products_by_serial (serial_number, product_name, purchase_date, warranty_months, customer_id, status, image_url) VALUES ('SP0029','LG TV 2018','2019-03-20',24,'CUST009','active','https://images.unsplash.com/photo-1517336714731-489689fd1ca8');
INSERT INTO products_by_serial (serial_number, product_name, purchase_date, warranty_months, customer_id, status, image_url) VALUES ('SP0030','Samsung TV 2019','2020-01-01',12,'CUST010','active','https://images.unsplash.com/photo-1519389950473-47ba0277781c');


INSERT INTO products_by_serial (serial_number, product_name, purchase_date, warranty_months, customer_id, status, image_url) VALUES ('SP0031','Xiaomi Robot Vacuum','2024-03-01',24,'CUST011','active','https://images.unsplash.com/photo-1632395629873-5c8c53a39b2b');
INSERT INTO products_by_serial (serial_number, product_name, purchase_date, warranty_months, customer_id, status, image_url) VALUES ('SP0032','Ecovacs Deebot','2023-09-15',36,'CUST012','active','https://images.unsplash.com/photo-1632071769794-ebf70f9137c5');
INSERT INTO products_by_serial (serial_number, product_name, purchase_date, warranty_months, customer_id, status, image_url) VALUES ('SP0033','Roborock S8','2024-02-10',24,'CUST013','active','https://images.unsplash.com/photo-1586789105297-03bb0a4b6e12');
INSERT INTO products_by_serial (serial_number, product_name, purchase_date, warranty_months, customer_id, status, image_url) VALUES ('SP0034','iRobot Roomba J7','2023-12-01',24,'CUST014','active','https://images.unsplash.com/photo-1580119056800-839e8a2c57d9');
INSERT INTO products_by_serial (serial_number, product_name, purchase_date, warranty_months, customer_id, status, image_url) VALUES ('SP0035','Xiaomi Vacuum Pro','2024-01-05',12,'CUST015','active','https://images.unsplash.com/photo-1606813853103-6d5c2c064b41');


INSERT INTO products_by_serial  (serial_number, product_name, purchase_date, warranty_months, customer_id, status, image_url) VALUES ('SP0036','Coway Water Filter','2021-01-15',24,'CUST016','active','https://images.unsplash.com/photo-1617957743092-855f18a64c29');
INSERT INTO products_by_serial  (serial_number, product_name, purchase_date, warranty_months, customer_id, status, image_url) VALUES ('SP0037','Panasonic Water Purifier','2020-07-10',24,'CUST017','active','https://images.unsplash.com/photo-1606813914529-4f50e1fbd6a2');
INSERT INTO products_by_serial  (serial_number, product_name, purchase_date, warranty_months, customer_id, status, image_url) VALUES ('SP0038','Karofi Water Filter','2020-03-05',36,'CUST018','active','https://images.unsplash.com/photo-1606813845750-bcae4dab2323');


INSERT INTO products_by_serial (serial_number, product_name, purchase_date, warranty_months, customer_id, status, image_url)VALUES ('SP0039','Coway Prime Water','2023-11-01',36,'CUST019','active','https://images.unsplash.com/photo-1557682224-5b8590cd9ec5');
INSERT INTO products_by_serial (serial_number, product_name, purchase_date, warranty_months, customer_id, status, image_url) VALUES ('SP0040','Panasonic Pure','2024-02-15',24,'CUST020','active','https://images.unsplash.com/photo-1570800927609-ba9d08a3a3be');


INSERT INTO products_by_serial (serial_number, product_name, purchase_date, warranty_months, customer_id, status, image_url) VALUES ('SP0041','Daikin Inverter','2024-01-01',24,'CUST001','active','https://images.unsplash.com/photo-1587202372775-98927e5eca9d');
INSERT INTO products_by_serial (serial_number, product_name, purchase_date, warranty_months, customer_id, status, image_url) VALUES ('SP0042','Panasonic AC','2023-10-20',36,'CUST002','active','https://images.unsplash.com/photo-1593642532973-d31b6557fa68');
INSERT INTO products_by_serial (serial_number, product_name, purchase_date, warranty_months, customer_id, status, image_url) VALUES ('SP0043','LG Dual Cool','2024-03-01',12,'CUST003','active','https://images.unsplash.com/photo-1616627520979-da189a93f81c');
INSERT INTO products_by_serial (serial_number, product_name, purchase_date, warranty_months, customer_id, status, image_url) VALUES ('SP0044','Daikin 2019','2020-02-10',12,'CUST004','active','https://images.unsplash.com/photo-1606813914547-cb28cac89912');
INSERT INTO products_by_serial (serial_number, product_name, purchase_date, warranty_months, customer_id, status, image_url) VALUES ('SP0045','Panasonic 2018','2019-05-01',24,'CUST005','active','https://images.unsplash.com/photo-1580119056800-839e8a2c57d9');
INSERT INTO products_by_serial (serial_number, product_name, purchase_date, warranty_months, customer_id, status, image_url) VALUES ('SP0046','LG 2020','2021-01-05',12,'CUST006','active','https://images.unsplash.com/photo-1587202372775-98927e5eca9d');
INSERT INTO products_by_serial (serial_number, product_name, purchase_date, warranty_months, customer_id, status, image_url) VALUES ('SP0047','Samsung WindFree','2023-07-10',24,'CUST007','active','https://images.unsplash.com/photo-1556905055-8f358a7a47b2');
INSERT INTO products_by_serial (serial_number, product_name, purchase_date, warranty_months, customer_id, status, image_url) VALUES ('SP0048','Mitsubishi AC','2022-06-10',12,'CUST008','active','https://images.unsplash.com/photo-1517336714731-489689fd1ca8');
INSERT INTO products_by_serial (serial_number, product_name, purchase_date, warranty_months, customer_id, status, image_url) VALUES ('SP0049','LG Air 2017','2018-03-15',36,'CUST009','active','https://images.unsplash.com/photo-1496181133206-80ce9b88a853');
INSERT INTO products_by_serial (serial_number, product_name, purchase_date, warranty_months, customer_id, status, image_url) VALUES ('SP0050','Daikin Classic','2019-09-10',24,'CUST010','active','https://images.unsplash.com/photo-1622464219605-7a6b690a5b3d');



-- Ticket đang chờ phân công
INSERT INTO warranty_tickets_by_product (serial_number, ticket_id, customer_id, created_at, issue_description, status, technician_id)
VALUES ('SP0001', 'WR0001', 'CUST001', toTimestamp(now()), 'Máy bị sập nguồn đột ngột', 'pending', null);

-- Ticket đã phân công nhưng chưa bắt đầu
INSERT INTO warranty_tickets_by_product (serial_number, ticket_id, customer_id, created_at, issue_description, status, technician_id)
VALUES ('SP0002', 'WR0002', 'CUST002', toTimestamp(now()), 'Không khởi động được', 'assigned', 'tech01');

-- Ticket đang được xử lý
INSERT INTO warranty_tickets_by_product (serial_number, ticket_id, customer_id, created_at, issue_description, status, technician_id)
VALUES ('SP0001', 'WR0003', 'CUST001', toTimestamp(now()), 'Màn hình bị sọc', 'in_progress', 'tech02');

-- Ticket đã hoàn thành
INSERT INTO warranty_tickets_by_product (serial_number, ticket_id, customer_id, created_at, issue_description, status, technician_id)
VALUES ('SP0002', 'WR0004', 'CUST002', toTimestamp(now()), 'Lỗi pin đã được thay', 'completed', 'tech01');

-- ========== DANH SÁCH KHÁCH HÀNG NGẪU NHIÊN ==========
INSERT INTO customers_by_id (customer_id, full_name, phone, email, address, status) VALUES ('CUST001','Nguyễn Văn An','0912345001','an.nguyen01@gmail.com','Hà Nội, Việt Nam','active');
INSERT INTO customers_by_id (customer_id, full_name, phone, email, address, status) VALUES ('CUST002','John Smith','0912345002','john.smith@gmail.com','New York, USA','active');
INSERT INTO customers_by_id (customer_id, full_name, phone, email, address, status) VALUES ('CUST003','Trần Thị Hoa','0912345003','hoa.tran03@gmail.com','TP. Hồ Chí Minh, Việt Nam','active');
INSERT INTO customers_by_id (customer_id, full_name, phone, email, address, status) VALUES ('CUST004','Sarah Johnson','0912345004','sarah.johnson@gmail.com','Los Angeles, USA','active');
INSERT INTO customers_by_id (customer_id, full_name, phone, email, address, status) VALUES ('CUST005','Lê Quốc Khánh','0912345005','khanh.le@gmail.com','Đà Nẵng, Việt Nam','active');
INSERT INTO customers_by_id (customer_id, full_name, phone, email, address, status) VALUES ('CUST006','Emily Davis','0912345006','emily.davis@gmail.com','London, UK','active');
INSERT INTO customers_by_id (customer_id, full_name, phone, email, address, status) VALUES ('CUST007','Phạm Hồng Sơn','0912345007','son.pham07@gmail.com','Cần Thơ, Việt Nam','active');
INSERT INTO customers_by_id (customer_id, full_name, phone, email, address, status) VALUES ('CUST008','Michael Brown','0912345008','michael.brown@gmail.com','Toronto, Canada','active');
INSERT INTO customers_by_id (customer_id, full_name, phone, email, address, status) VALUES ('CUST009','Nguyễn Thị Minh','0912345009','minh.nguyen09@gmail.com','Bình Dương, Việt Nam','active');
INSERT INTO customers_by_id (customer_id, full_name, phone, email, address, status) VALUES ('CUST010','David Wilson','0912345010','david.wilson@gmail.com','Sydney, Australia','active');

INSERT INTO customers_by_id (customer_id, full_name, phone, email, address, status) VALUES ('CUST011','Trương Mỹ Linh','0912345011','linh.truong11@gmail.com','Nha Trang, Việt Nam','active');
INSERT INTO customers_by_id (customer_id, full_name, phone, email, address, status) VALUES ('CUST012','Jessica Lee','0912345012','jessica.lee@gmail.com','Seoul, Korea','active');
INSERT INTO customers_by_id (customer_id, full_name, phone, email, address, status) VALUES ('CUST013','Hoàng Gia Huy','0912345013','huy.hoang@gmail.com','Hải Phòng, Việt Nam','active');
INSERT INTO customers_by_id (customer_id, full_name, phone, email, address, status) VALUES ('CUST014','Oliver Taylor','0912345014','oliver.taylor@gmail.com','Auckland, New Zealand','active');
INSERT INTO customers_by_id (customer_id, full_name, phone, email, address, status) VALUES ('CUST015','Nguyễn Thị Thu','0912345015','thu.nguyen@gmail.com','Huế, Việt Nam','active');
INSERT INTO customers_by_id (customer_id, full_name, phone, email, address, status) VALUES ('CUST016','William Harris','0912345016','william.harris@gmail.com','Chicago, USA','active');
INSERT INTO customers_by_id (customer_id, full_name, phone, email, address, status) VALUES ('CUST017','Đặng Đức Bình','0912345017','binh.dang@gmail.com','TP. Biên Hòa, Việt Nam','active');
INSERT INTO customers_by_id (customer_id, full_name, phone, email, address, status) VALUES ('CUST018','Sophia Martinez','0912345018','sophia.martinez@gmail.com','Madrid, Spain','active');
INSERT INTO customers_by_id (customer_id, full_name, phone, email, address, status) VALUES ('CUST019','Phan Thanh Tâm','0912345019','tam.phan@gmail.com','Buôn Ma Thuột, Việt Nam','active');
INSERT INTO customers_by_id (customer_id, full_name, phone, email, address, status) VALUES ('CUST020','James Anderson','0912345020','james.anderson@gmail.com','Berlin, Germany','active');


INSERT INTO customers_by_phone (phone, customer_id, full_name, email, address, status) VALUES ('0912345001','CUST001','Nguyễn Văn An','an.nguyen01@gmail.com','Hà Nội, Việt Nam','active');
INSERT INTO customers_by_phone (phone, customer_id, full_name, email, address, status) VALUES ('0912345002','CUST002','John Smith','john.smith@gmail.com','New York, USA','active');
INSERT INTO customers_by_phone (phone, customer_id, full_name, email, address, status) VALUES ('0912345003','CUST003','Trần Thị Hoa','hoa.tran03@gmail.com','TP. Hồ Chí Minh, Việt Nam','active');
INSERT INTO customers_by_phone (phone, customer_id, full_name, email, address, status) VALUES ('0912345004','CUST004','Sarah Johnson','sarah.johnson@gmail.com','Los Angeles, USA','active');
INSERT INTO customers_by_phone (phone, customer_id, full_name, email, address, status) VALUES ('0912345005','CUST005','Lê Quốc Khánh','khanh.le@gmail.com','Đà Nẵng, Việt Nam','active');
INSERT INTO customers_by_phone (phone, customer_id, full_name, email, address, status) VALUES ('0912345006','CUST006','Emily Davis','emily.davis@gmail.com','London, UK','active');
INSERT INTO customers_by_phone (phone, customer_id, full_name, email, address, status) VALUES ('0912345007','CUST007','Phạm Hồng Sơn','son.pham07@gmail.com','Cần Thơ, Việt Nam','active');
INSERT INTO customers_by_phone (phone, customer_id, full_name, email, address, status) VALUES ('0912345008','CUST008','Michael Brown','michael.brown@gmail.com','Toronto, Canada','active');
INSERT INTO customers_by_phone (phone, customer_id, full_name, email, address, status) VALUES ('0912345009','CUST009','Nguyễn Thị Minh','minh.nguyen09@gmail.com','Bình Dương, Việt Nam','active');
INSERT INTO customers_by_phone (phone, customer_id, full_name, email, address, status) VALUES ('0912345010','CUST010','David Wilson','david.wilson@gmail.com','Sydney, Australia','active');

INSERT INTO customers_by_phone (phone, customer_id, full_name, email, address, status) VALUES ('0912345011','CUST011','Trương Mỹ Linh','linh.truong11@gmail.com','Nha Trang, Việt Nam','active');
INSERT INTO customers_by_phone (phone, customer_id, full_name, email, address, status) VALUES ('0912345012','CUST012','Jessica Lee','jessica.lee@gmail.com','Seoul, Korea','active');
INSERT INTO customers_by_phone (phone, customer_id, full_name, email, address, status) VALUES ('0912345013','CUST013','Hoàng Gia Huy','huy.hoang@gmail.com','Hải Phòng, Việt Nam','active');
INSERT INTO customers_by_phone (phone, customer_id, full_name, email, address, status) VALUES ('0912345014','CUST014','Oliver Taylor','oliver.taylor@gmail.com','Auckland, New Zealand','active');
INSERT INTO customers_by_phone (phone, customer_id, full_name, email, address, status) VALUES ('0912345015','CUST015','Nguyễn Thị Thu','thu.nguyen@gmail.com','Huế, Việt Nam','active');
INSERT INTO customers_by_phone (phone, customer_id, full_name, email, address, status) VALUES ('0912345016','CUST016','William Harris','william.harris@gmail.com','Chicago, USA','active');
INSERT INTO customers_by_phone (phone, customer_id, full_name, email, address, status) VALUES ('0912345017','CUST017','Đặng Đức Bình','binh.dang@gmail.com','TP. Biên Hòa, Việt Nam','active');
INSERT INTO customers_by_phone (phone, customer_id, full_name, email, address, status) VALUES ('0912345018','CUST018','Sophia Martinez','sophia.martinez@gmail.com','Madrid, Spain','active');
INSERT INTO customers_by_phone (phone, customer_id, full_name, email, address, status) VALUES ('0912345019','CUST019','Phan Thanh Tâm','tam.phan@gmail.com','Buôn Ma Thuột, Việt Nam','active');
INSERT INTO customers_by_phone (phone, customer_id, full_name, email, address, status) VALUES ('0912345020','CUST020','James Anderson','james.anderson@gmail.com','Berlin, Germany','active');


INSERT INTO repairs_by_technician (technician_id, repair_id, ticket_id, serial_number, start_date, complete_date, repair_description, parts_used, cost, status)
VALUES ('tech01', 'REPAIR001', 'WR0001', 'SP0001', '2025-01-15', '2025-01-20', 'Lỗi nguồn', 'Thay nguồn', 1500000, 'completed');

INSERT INTO repairs_by_technician (technician_id, repair_id, ticket_id, serial_number, start_date, complete_date, repair_description, parts_used, cost, status) VALUES ('tech02','REPAIR002','WR0002','SP0002','2025-02-15','2025-02-20','Lỗi màn hình','Thay màn hình',2500000,'completed');

INSERT INTO repairs_by_technician (technician_id, repair_id, ticket_id, serial_number, start_date, complete_date, repair_description, parts_used, cost, status) VALUES ('tech01','REPAIR003','WR0003','SP0003','2025-03-15','2025-03-20','Lỗi pin','Thay pin',1800000,'completed');

INSERT INTO repairs_by_technician (technician_id, repair_id, ticket_id, serial_number, start_date, complete_date, repair_description, parts_used, cost, status) VALUES ('tech03','REPAIR004','WR0004','SP0004','2025-04-15','2025-04-20','Lỗi main','Sửa main',3500000,'completed');

INSERT INTO repairs_by_technician (technician_id, repair_id, ticket_id, serial_number, start_date, complete_date, repair_description, parts_used, cost, status) VALUES ('tech01','REPAIR005','WR0005','SP0005','2025-05-15','2025-05-20','Lỗi phần mềm','Cài lại phần mềm',900000,'completed');

INSERT INTO repairs_by_technician (technician_id, repair_id, ticket_id, serial_number, start_date, complete_date, repair_description, parts_used, cost, status) VALUES ('tech02','REPAIR006','WR0006','SP0006','2025-06-15','2025-06-20','Lỗi loa','Thay loa',1200000,'completed');

INSERT INTO repairs_by_technician (technician_id, repair_id, ticket_id, serial_number, start_date, complete_date, repair_description, parts_used, cost, status) VALUES ('tech03','REPAIR007','WR0007','SP0007','2025-07-15','2025-07-20','Lỗi camera','Thay camera',1400000,'completed');

INSERT INTO repairs_by_technician (technician_id, repair_id, ticket_id, serial_number, start_date, complete_date, repair_description, parts_used, cost, status) VALUES ('tech01','REPAIR008','WR0008','SP0008','2025-08-15','2025-08-20','Lỗi bàn phím','Thay bàn phím',1000000,'completed');

INSERT INTO repairs_by_technician (technician_id, repair_id, ticket_id, serial_number, start_date, complete_date, repair_description, parts_used, cost, status) VALUES ('tech02','REPAIR009','WR0009','SP0009','2025-09-15','2025-09-20','Lỗi cổng sạc','Thay cổng sạc',1100000,'completed');

INSERT INTO repairs_by_technician (technician_id, repair_id, ticket_id, serial_number, start_date, complete_date, repair_description, parts_used, cost, status) VALUES ('tech03','REPAIR010','WR0010','SP0010','2025-10-15','2025-10-20','Lỗi wifi','Thay wifi',1600000,'completed');

INSERT INTO repairs_by_technician (technician_id, repair_id, ticket_id, serial_number, start_date, complete_date, repair_description, parts_used, cost, status) VALUES ('tech01','REPAIR011','WR0011','SP0011','2025-11-15','2025-11-20','Lỗi CPU','Sửa CPU',4200000,'completed');

INSERT INTO repairs_by_technician (technician_id, repair_id, ticket_id, serial_number, start_date, complete_date, repair_description, parts_used, cost, status) VALUES ('tech02','REPAIR012','WR0012','SP0012','2025-12-15','2025-12-20','Lỗi GPU','Thay GPU',5000000,'completed');

INSERT INTO repairs_by_technician (technician_id, repair_id, ticket_id, serial_number, start_date, complete_date, repair_description, parts_used, cost, status) VALUES ('tech03','REPAIR013','WR0013','SP0013','2025-03-15','2025-03-20','Lỗi tản nhiệt','Thay quạt',1300000,'completed');

INSERT INTO repairs_by_technician (technician_id, repair_id, ticket_id, serial_number, start_date, complete_date, repair_description, parts_used, cost, status) VALUES ('tech02','REPAIR014','WR0014','SP0014','2025-04-15','2025-04-20','Lỗi màn hình phụ','Thay màn phụ',2100000,'completed');

INSERT INTO repairs_by_technician (technician_id, repair_id, ticket_id, serial_number, start_date, complete_date, repair_description, parts_used, cost, status) VALUES ('tech01','REPAIR015','WR0015','SP0015','2025-05-15','2025-05-20','Lỗi cảm biến','Thay cảm biến',1750000,'completed');




