class AlarmController < ApplicationController
  def index
    list
    render :action => 'list'
  end

  # GETs should be safe (see http://www.w3.org/2001/tag/doc/whenToUseGet.html)
  verify :method => :post, :only => [ :destroy, :create, :update ],
         :redirect_to => { :action => :list }

  def list
    @alarm_pages, @alarms = paginate :alarms, :per_page => 10
  end

  def show
    @alarm = Alarm.find(params[:id])
  end

  def new
    @alarm = Alarm.new
  end

  def create
    @alarm = Alarm.new(params[:alarm])
    if @alarm.save
      write_to_xml
      flash[:notice] = 'Alarm was successfully created.'
      redirect_to :action => 'list'
    else
      render :action => 'new'
    end
  end

  def edit
    @alarm = Alarm.find(params[:id])
  end

  def update
    @alarm = Alarm.find(params[:id])
    if @alarm.update_attributes(params[:alarm])
      write_to_xml
      flash[:notice] = 'Alarm was successfully updated.'
      redirect_to :action => 'show', :id => @alarm
    else
      render :action => 'edit'
    end
  end

  def destroy
    Alarm.find(params[:id]).destroy
    write_to_xml
    redirect_to :action => 'list'
  end

  private
  def write_to_xml
    File.open('public/schedule2.xml', 'w') { |file|
      file.write "<schedule>\n"
      Alarm.find(:all, :order => 'id').each { |alarm|
        file.write "  <alarm name='#{alarm.name}' datetime='#{DateTime.parse(alarm.datetime).strftime('%Y-%m-%d %H:%M:%S')}'/>\n"
      }
      file.write "</schedule>\n"
    }
    FileUtils.mv 'public/schedule2.xml', 'public/schedule.xml'
  end
end
