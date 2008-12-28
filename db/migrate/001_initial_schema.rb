class InitialSchema < ActiveRecord::Migration
  def self.up
    execute "create table events (
      id int auto_increment not null,
      short_name varchar(255) not null,
      primary key (id)
    );"
    execute "create table alarms (
      id int auto_increment not null,
      event_id int not null,
      datetime varchar(16) not null,
      primary key (id)
    );"
  end
  def self.down
    drop_table 'events'
    drop_table 'alarms'
  end
end
