require File.dirname(__FILE__) + '/../test_helper'
require 'alarm_controller'

# Re-raise errors caught by the controller.
class AlarmController; def rescue_action(e) raise e end; end

class AlarmControllerTest < Test::Unit::TestCase
  fixtures :alarms

  def setup
    @controller = AlarmController.new
    @request    = ActionController::TestRequest.new
    @response   = ActionController::TestResponse.new

    @first_id = alarms(:first).id
  end

  def test_index
    get :index
    assert_response :success
    assert_template 'list'
  end

  def test_list
    get :list

    assert_response :success
    assert_template 'list'

    assert_not_nil assigns(:alarms)
  end

  def test_show
    get :show, :id => @first_id

    assert_response :success
    assert_template 'show'

    assert_not_nil assigns(:alarm)
    assert assigns(:alarm).valid?
  end

  def test_new
    get :new

    assert_response :success
    assert_template 'new'

    assert_not_nil assigns(:alarm)
  end

  def test_create
    num_alarms = Alarm.count

    post :create, :alarm => {}

    assert_response :redirect
    assert_redirected_to :action => 'list'

    assert_equal num_alarms + 1, Alarm.count
  end

  def test_edit
    get :edit, :id => @first_id

    assert_response :success
    assert_template 'edit'

    assert_not_nil assigns(:alarm)
    assert assigns(:alarm).valid?
  end

  def test_update
    post :update, :id => @first_id
    assert_response :redirect
    assert_redirected_to :action => 'show', :id => @first_id
  end

  def test_destroy
    assert_nothing_raised {
      Alarm.find(@first_id)
    }

    post :destroy, :id => @first_id
    assert_response :redirect
    assert_redirected_to :action => 'list'

    assert_raise(ActiveRecord::RecordNotFound) {
      Alarm.find(@first_id)
    }
  end
end
