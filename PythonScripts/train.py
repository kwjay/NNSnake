import numpy as np
from agent import Agent

class UnityTrainer:
    def __init__(self):
        self.agent = Agent()
        self.state = None
        self.action = None
        self.highscore = 0

    def step(self, data):
        new_state = np.array(data.get("state"), dtype=int)
        if self.state is None:
            self.state = new_state
            self.action = self.agent.get_action(self.state)
            return { "action": self.action }

        collision = bool(data.get("collision", False))
        food_eaten = bool(data.get("foodEaten", False))
        done = data.get("done")
        score = data.get("score", 0)
        reward = 0
        reward += 10 if food_eaten else 0
        reward -= 10 if collision else 0

        self.agent.train_short_memory(self.state, self.action, reward, new_state, done)
        self.agent.remember(self.state, self.action, reward, new_state, done)

        self.state = new_state
        self.action = self.agent.get_action(self.state)

        if done:
            self.agent.n_games += 1
            self.agent.train_long_memory()

            if score > self.highscore:
                self.highscore = score
                self.agent.model.save()

            print(
                f"Game {self.agent.n_games} | "
                f"Score: {score} | "
                f"Record: {self.highscore} | "
                f"Epsilon: {self.agent.epsilon:.3f}"
            )

        return { "action": self.action }
