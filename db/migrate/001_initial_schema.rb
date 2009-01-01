class InitialSchema < ActiveRecord::Migration
  def self.up
    execute "create table alarms (
      id int auto_increment not null,
      datetime varchar(16) not null,
      name varchar(255) not null,
      primary key (id)
    );"
  end
  def self.down
    drop_table 'alarms'
  end
end
