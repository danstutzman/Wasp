class Event < ActiveRecord::Base
  has_many :alarms
end
