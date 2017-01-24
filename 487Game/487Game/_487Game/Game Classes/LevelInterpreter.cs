using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace _487Game
{
  /// <summary>
  /// This class is used to load enemy information from xml level configuration files
  /// </summary>
  class LevelInterpreter
  {
    List<XDocument> levelDocs;
    EnemyGroupFactory _factory;
    int currentLevel;
    ContentManager _content;

    public LevelInterpreter(string dirPath, EnemyGroupFactory factory, ContentManager content)
    {
      levelDocs = Directory.GetFiles(dirPath, "*.xml").Select(x => XDocument.Load(x)).ToList();
      _factory = factory;
      currentLevel = 0;
      _content = content;
    }
    
    /// <summary>
    /// This method loads the groups from next available level into memory
    /// </summary>
    /// <returns></returns>
    public List<Tuple<int, List<Enemy>>> LoadLevel()
    {
      var groups = levelDocs[currentLevel++].Descendants("Group");
      var groupList = new List<Tuple<int, List<Enemy>>>();

      foreach (var group in groups)
      {
        groupList.Add(CreateGroup(group));
      }

      return groupList;
    }

    /// <summary>
    /// This method creates a single enemy group from a XElement named group
    /// </summary>
    /// <param name="group"></param>
    /// <returns></returns>
    private Tuple<int, List<Enemy>> CreateGroup(XElement group)
    {
      // Get Enemies XElement
      XElement enemyEl = group.Element("Enemies");
      // Create new Enemy group tuple with spawn time
      var gr = new Tuple<int, List<Enemy>>(int.Parse(group.Element("SpawnTime").Value), new List<Enemy>());
      // Get Bullets XElement
      XElement bulletEl = group.Element("Bullets");
      // Creates initial position vector from SpawnLocation element and x and y attributes (Required)
      Vector2 initPos = new Vector2(float.Parse(enemyEl.Element("SpawnLocation").Attribute("x").Value), float.Parse(enemyEl.Element("SpawnLocation").Attribute("y").Value));
      // Number of enemies element (Defaults to 1)
      int number = enemyEl.Element("Number") != null ? int.Parse(enemyEl.Element("Number")?.Value) : 1;
      // Spacing of enemies element (Defaults to 0)
      int spacing = enemyEl.Element("Spacing") != null ? int.Parse(enemyEl.Element("Spacing")?.Value) : 0;
      // XShift element (defaults to 0, shifts x spawn posiiton of each enemy)
      int xshift = enemyEl.Element("XShift") != null ? int.Parse(enemyEl.Element("XShift")?.Value) : 0;
      // Health element (defaults to 10)
      int health = enemyEl.Element("Health") != null ? int.Parse(enemyEl.Element("Health")?.Value) : 10;
      // XRand element, if exists, randomizes x spawn location
      bool xrand = enemyEl.Element("XRand") != null ? true : false;

      // <Group>
      //    <SpawnTime>16</SpawnTime>                                 -> goes into gr tuple
      //    <Enemies>                                                 -> enemyEl
      //	    <Number>1</Number>                                      -> number
      //	    <Spacing>20</Spacing>                                   -> Spacing
      //	    <SpawnLocation x="400" y="-50" />                       -> initPos
      //	    <XShift>0</XShift>                                      -> xshift
      //	    <Movements>                                             -> Movements element for enemies
      //		    <Movement angle="1.58" duration="60" />               -> Movement element
      //		    <Freeze time="50"/>                                   -> Movement element
      //		    <Movement angle="3" duration="400" speed="60" />      -> Movement element
      //	    </Movements>
      //    </Enemies>
      //    <Bullets>                                                 -> bulletEl
      //	    <Bullet>                                                -> Bullet XElement
      //		    <Time>80</Time>                                       -> Bullet spawn time
      //		    <Movements>                                           -> Movements element for bullets
      //			    <Movement angle="1.58" duration="500" />            -> Movement element
      //		    </Movements>
      //	    </Bullet>
      //    </Bullets>
      // </Group>

      // Check for boss type
      if (group.Element("Boss") != null && group.Element("Boss").Value == "True")
      {
        // Get final boss
        gr.Item2.AddRange(_factory.CreateFinalBoss(CreateMovementQueue(enemyEl.Element("Movements"), 1)[0], initPos, GetBossAttackTypeList(bulletEl)));
      }
      else
      {
        // Create regular group of enemies
        gr.Item2.AddRange(_factory.CreateGroup(CreateMovementQueue(enemyEl.Element("Movements"), number), bulletEl != null? GetEnemyBullets(bulletEl, number) : null, initPos, number, spacing, xshift, xrand, health));
      }

      return gr;
    }

    /// <summary>
    /// Creates an array of movement queues to assign to group of enemies or bullets
    /// </summary>
    /// <param name="movements">Movements XElement</param>
    /// <param name="number">number of enemies/bullets</param>
    /// <returns>movement queue array</returns>
    private Queue<Movement>[] CreateMovementQueue(XElement movements, int number)
    {
      var queueArr = new Queue<Movement>[number];

      for(int i = 0; i < number; i++)
      {
        queueArr[i] = new Queue<Movement>();
      }

      foreach (var movement in movements.Elements())
      {
        foreach (var queue in queueArr)
        {
          queue.Enqueue(GetMovement(movement));
        }
      }

      return queueArr;
    }

    /// <summary>
    /// Parse out movement parameters and create movement object
    /// </summary>
    /// <param name="movement">movement XElement</param>
    /// <returns>movement object</returns>
    private Movement GetMovement(XElement movement)
    {
      // First check if freeze movement
      if(movement.Name == "Freeze")
      {
        return new Movement(0, int.Parse(movement.Attribute("time")?.Value), 0, 0);
      }

      // These are all the posible attributes for the movement element
      float angle = movement.Attribute("angle") != null ? float.Parse(movement.Attribute("angle").Value) : float.NaN;
      int duration = movement.Attribute("duration") != null ? int.Parse(movement.Attribute("duration").Value) : -1;
      float turningAngle = movement.Attribute("turnangle") != null ? float.Parse(movement.Attribute("turnangle").Value) : 0;
      float speed = movement.Attribute("speed") != null ? float.Parse(movement.Attribute("speed").Value) : 200;
      float acc = movement.Attribute("turnacc") != null ? float.Parse(movement.Attribute("turnacc").Value) : 1;

      return new Movement(angle, duration, turningAngle, speed, acc);
    }

    /// <summary>
    /// Gets queue representing sequence of boss attacks
    /// </summary>
    /// <param name="bullets">bullets XElement</param>
    /// <returns>Queue of tuples of spawn times and chars symbolizing attacks</returns>
    private Queue<Tuple<int, char>> GetBossAttackTypeList(XElement bullets)
    {
      Queue<Tuple<int, char>> bulletQueue = new Queue<Tuple<int, char>>();

      foreach(var attack in bullets.Elements("BulletGroup"))
      {
        bulletQueue.Enqueue(new Tuple<int, char>(int.Parse(attack.Attribute("time").Value), char.Parse(attack.Attribute("type").Value)));
      }

      return bulletQueue;
    }

    /// <summary>
    /// Gets Queue array of enemy projectiles to assign to each enemy in a group
    /// </summary>
    /// <param name="bullets">bullets XElement</param>
    /// <param name="number">number of enemies in a group</param>
    /// <returns>queue array of enemy projectiles and spawn times</returns>
    private Queue<Tuple<Projectile, int>>[] GetEnemyBullets(XElement bullets, int number)
    {
      Queue<Tuple<Projectile, int>>[] bulletQueue = new Queue<Tuple<Projectile, int>>[number];

      for(int i = 0; i < number; i++)
      {
        bulletQueue[i] = new Queue<Tuple<Projectile, int>>();
      }

      foreach (var bullet in bullets.Elements("Bullet"))
      {
        foreach (var enemy in bulletQueue)
        {
          enemy.Enqueue(new Tuple<Projectile, int>(new BasicProjectile(BulletFactory.Instance().textureContainer.defaultProjectile, 0, 0, CreateMovementQueue(bullet.Element("Movements"), number)[0]), int.Parse(bullet.Element("Time").Value)));
        }
      }

      return bulletQueue;
    }
  }
}
