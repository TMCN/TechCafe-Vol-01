#include "cinder/app/AppNative.h"
#include "cinder/gl/gl.h"
#include "cinder/Camera.h"

#include "Leap.h"

using namespace ci;
using namespace ci::app;
using namespace std;

class CinderSampleApp : public AppNative {
  public:
	void setup();
	void mouseDown( MouseEvent event );	
	void update();
	void draw();

  Vec3f toVec3f( Leap::Vector vec )
  {
    return Vec3f( vec.x, vec.y, vec.z );
  }

  CameraPersp mCam;
  Leap::Controller mLeap;
};

void CinderSampleApp::setup()
{
  // �E�B���h�E�̈ʒu�ƃT�C�Y��ݒ�
  setWindowPos(50, 50);
  setWindowSize(1280, 700);

  // �J����(���_)�̐ݒ�
  float y = 250;
  mCam.setPerspective( 60.0f, getWindowAspectRatio(), 5.0f, 3000.0f );
  mCam.lookAt( Vec3f( 0.0f, y, 500.0f ), Vec3f( 0.0f, y, 0.0f ), Vec3f( 0.0f, 1.0f, 0.0f ) );

  // �`�掞�ɉ��s���̍l����L���ɂ���
  gl::enableDepthRead();

  // ������ǉ�����
  glEnable( GL_LIGHTING );
  glEnable( GL_LIGHT0 );
}

void CinderSampleApp::mouseDown( MouseEvent event )
{
}

void CinderSampleApp::update()
{
}

void CinderSampleApp::draw()
{
	// clear out the window with black
	gl::clear( Color( 0, 0, 0 ) ); 
  gl::setMatrices( mCam );

  // �w�̈ʒu��\������
  auto frame = mLeap.frame();
  for ( auto finger : frame.fingers() ) {
    gl::drawSphere( toVec3f( finger.tipPosition() ), 10 );
  }
}

CINDER_APP_NATIVE( CinderSampleApp, RendererGl )
