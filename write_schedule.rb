require File.dirname(__FILE__) + '/config/environment'

File.open('public/schedule2.xml', 'w') { |file|
  file.write "<schedule>\n"
  Event.find(:all, :order => 'id').each { |event|
    file.write "  <event short_name='#{event.short_name}'>\n"
    event.alarms.each { |alarm|
      file.write "    <alarm datetime='#{DateTime.parse(alarm.datetime).strftime('%Y-%m-%d %H:%M:%S')}'/>\n"
    }
    file.write "  </event>\n"
  }
  file.write "</schedule>\n"
}
FileUtils.mv 'public/schedule2.xml', 'public/schedule.xml'
