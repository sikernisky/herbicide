def generate_spawn_schedule(enemy_name, stages, enemies_per_wave, wave_gap, time_between_enemies, first_wave_time):
    """
    Generate a spawn schedule for enemies in the specified format.

    :param enemy_name: The base name of the enemy.
    :param stages: A list of integers representing the number of enemies per stage.
    :param enemies_per_wave: A list of lists where each sublist specifies the number of enemies per wave for a stage.
    :param wave_gap: The time gap between waves (in seconds).
    :param time_between_enemies: The time between enemies in a wave (in seconds).
    :param first_wave_time: The time when the first wave starts (in seconds).
    :return: A formatted string with the spawn schedule.
    """
    schedule = []

    for stage_index, (num_enemies, wave_distribution) in enumerate(zip(stages, enemies_per_wave)):
        if sum(wave_distribution) != num_enemies:
            raise ValueError(f"The sum of enemies per wave for stage {stage_index} does not match the total enemies specified.")

        current_time = first_wave_time

        for wave_size in wave_distribution:
            for enemy_index in range(wave_size):
                spawn_time = current_time + enemy_index * time_between_enemies
                schedule.append(f"{enemy_name}{stage_index}-{spawn_time:.2f}")

            current_time += wave_gap

    return ", ".join(schedule)

# Example input
stages = [8, 12, 16, 24]                      # Enemies per stage
enemies_per_wave = [[2, 2, 4], [3, 3, 3, 3], [4, 4, 4, 4], [4, 4, 4, 4, 4, 4]]  # Enemies per wave for each stage
wave_gap = 15                                 # Time between waves
time_between_enemies = 2                      # Time between enemies in a wave
first_wave_time = 5                           # First wave start time

# Generate the spawn schedule
spawn_schedule = generate_spawn_schedule("kudzu", stages, enemies_per_wave, wave_gap, time_between_enemies, first_wave_time)
print(spawn_schedule)
