drop table if exists alarms;
create table alarms (
      id int auto_increment not null,
      armed tinyint(1) not null,
      datetime varchar(16) not null,
      name varchar(255) not null,
      updated_at datetime not null,
      primary key (id)
    );
insert into alarms (armed, datetime, name, updated_at) values (1, '20090101000000', 'test', now());
