"""
Llama server package
"""

import os
import json
import asyncio
import websockets
from dotenv import load_dotenv
from gpt4all import GPT4All


class LlamaServer:
    """
    Websocket server that receives a text and returns a generated text
    """

    def __init__(self, host, port, model, generation_parameters):
        self.host = host
        self.port = port
        self.model = GPT4All(model)
        self.generation_parameters = generation_parameters

    async def handle_client(self, websocket, _):
        """
        Manage the connection with the client and the model
        """
        print("Conection stablished")
        try:
            async for event in websocket:
                print(f"Event getted: {event}")
                response = self.model.generate(event, **self.generation_parameters)
                await websocket.send(response)
                print(f"Response send: {response}")
        except websockets.exceptions.ConnectionClosedOK:
            print("Conection closed")

    async def main(self):
        """
        Create the server and wait for connections
        """
        server = await websockets.serve(self.handle_client, self.host, self.port)
        print(f"Listen in: {self.host}:{self.port}")
        await server.wait_closed()


if __name__ == "__main__":
    load_dotenv()

    HOST = os.getenv("HOST")
    PORT = int(os.getenv("PORT"))
    MODEL = os.getenv("MODEL")
    EXTRA_INFO = json.loads(os.getenv("EXTRA_INFO"))

    llama_server = LlamaServer(
        HOST,
        PORT,
        MODEL,
        EXTRA_INFO,
    )
    asyncio.run(llama_server.main())
