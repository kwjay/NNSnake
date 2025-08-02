import asyncio
import websockets
import json
from train import UnityTrainer


async def handle_connection(websocket):
    print("Client connected")
    unity_trainer = UnityTrainer()
    try:
        async for message in websocket:
            game_info = json.loads(message)
            direction = unity_trainer.step(game_info)
            await websocket.send(json.dumps(direction))
    except websockets.exceptions.ConnectionClosed:
        print("Connection closed")


async def main():
    print("Server started on ws://localhost:8080")
    async with websockets.serve(handle_connection, "localhost", 8080):
        await asyncio.Future()


if __name__ == "__main__":
    asyncio.run(main())
