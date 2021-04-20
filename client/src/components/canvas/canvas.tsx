import './canvas.scss';

export const Canvas = () => {
  return (
    <div
      id='canvasContainerOuter'
      className='canvasContainer'
      style={{
        width: '100vw',
        height: '20vh',
        display: 'flex',
        justifyContent: 'center',
        margin: '20px',
      }}
    >
      <div id='canvasContainerInner'>
        <div id='canvasContainer'></div>

        {/* DONT THINK THIS IS RELEVANT ? */}
        {/* <div id='guiContainer'>
          <div id='guiTable'></div>

          <div id='constantMenu'></div>
        </div> */}
      </div>
    </div>
  );
};
