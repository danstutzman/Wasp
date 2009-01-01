delete from events;
delete from alarms;
insert into events (short_name) values ('Accompany at church');
select last_insert_id() into @event_id;
insert into alarms (event_id, datetime) values (@event_id, '200901030930');
