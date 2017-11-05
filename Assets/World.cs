﻿using System.Collections;
using System.Collections.Generic;
using Assets.Generation;
using UnityEngine;

public class World : MonoBehaviour {

	public GameObject Player;
	public readonly Dictionary<Vector3, Chunk> Chunks = new Dictionary<Vector3, Chunk>();
	private MeshQueue _meshQueue;
    private GenerationQueue _generationQueue;

	void Awake(){
		_meshQueue = new MeshQueue (this);
		_generationQueue = new GenerationQueue (this);
	}

	public void AddToQueue(Chunk Chunk, bool DoMesh)
	{
		if (DoMesh) {
			_meshQueue.Add (Chunk);
		} else {
			_generationQueue.Queue.Add (Chunk);
		}
	}

    public void AddChunk(Vector3 Offset, Chunk Chunk)
    {
        if (!this.Chunks.ContainsKey(Offset))
        {
            this.Chunks.Add(Offset, Chunk);
			this._generationQueue.Queue.Add(Chunk);
        }
    }
    public void RemoveChunk(Chunk Chunk) { 
		lock(Chunks){
			if (Chunks.ContainsKey (Chunk.Position))
				Chunks.Remove (Chunk.Position);
		}
		Chunk.Dispose ();
	}

	public bool ContainsMeshQueue(Chunk chunk){
		lock(_meshQueue) return _meshQueue.Contains(chunk);
	}

	public bool ContainsGenerationQueue(Chunk chunk){
		lock(_generationQueue) return _generationQueue.Queue.Contains(chunk);
	}

    public Vector3 ToBlockSpace(Vector3 Vec3){
		
		int ChunkX = (int) Vec3.x >> Chunk.Bitshift;
		int ChunkY = (int) Vec3.y >> Chunk.Bitshift;
		int ChunkZ = (int) Vec3.z >> Chunk.Bitshift;
			
		ChunkX *= Chunk.ChunkSize;
		ChunkY *= Chunk.ChunkSize;
		ChunkZ *= Chunk.ChunkSize;
			
		int X = (int) Mathf.Floor( (Vec3.x - ChunkX) / Chunk.ChunkSize );
		int Y = (int) Mathf.Floor( (Vec3.y - ChunkX) / Chunk.ChunkSize );
		int Z = (int) Mathf.Floor( (Vec3.z - ChunkZ) / Chunk.ChunkSize );
			
		return new Vector3(X, Y ,Z);
	}
		
	public Chunk GetChunkAt(Vector3 Vec3){
		int ChunkX = (int) Vec3.x >> Chunk.Bitshift;
		int ChunkY = (int) Vec3.y >> Chunk.Bitshift;
		int ChunkZ = (int) Vec3.z >> Chunk.Bitshift;
			
		ChunkX *= Chunk.ChunkSize;
		ChunkY *= Chunk.ChunkSize;
		ChunkZ *= Chunk.ChunkSize;
			
		return this.GetChunkByOffset(ChunkX, ChunkY, ChunkZ);
	}
		
	public Vector3 ToChunkSpace(Vector3 Vec3){
		int ChunkX = (int) Vec3.x >> Chunk.Bitshift;
		int ChunkY = (int) Vec3.y >> Chunk.Bitshift;
		int ChunkZ = (int) Vec3.z >> Chunk.Bitshift;

		ChunkX *= Chunk.ChunkSize;
		ChunkY *= Chunk.ChunkSize;
		ChunkZ *= Chunk.ChunkSize;	
		
		return new Vector3(ChunkX, ChunkY, ChunkZ);
	}
	public Chunk GetChunkByOffset(float X, float Y, float Z) {
		return this.GetChunkByOffset (new Vector3(X,Y,Z));
	}
    public Chunk GetChunkByOffset(Vector3 Offset) {
		lock(Chunks){
			if (Chunks.ContainsKey (Offset))
				return Chunks [Offset];
		}
		return null;
	}

	public bool Discard{
		get{ return (_meshQueue != null) ? _meshQueue.Discard : true; }
	}

}