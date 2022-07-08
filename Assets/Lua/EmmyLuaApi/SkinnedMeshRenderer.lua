﻿---@class SkinnedMeshRenderer : Renderer
---@field public quality number
---@field public updateWhenOffscreen bool
---@field public forceMatrixRecalculationPerRender bool
---@field public rootBone Transform
---@field public bones Transform[]
---@field public sharedMesh Mesh
---@field public skinnedMotionVectors bool
---@field public localBounds Bounds
local SkinnedMeshRenderer={ }
---@public
---@param index number
---@return number
function SkinnedMeshRenderer:GetBlendShapeWeight(index) end
---@public
---@param index number
---@param value number
---@return void
function SkinnedMeshRenderer:SetBlendShapeWeight(index, value) end
---@public
---@param mesh Mesh
---@return void
function SkinnedMeshRenderer:BakeMesh(mesh) end
---@public
---@param mesh Mesh
---@param useScale bool
---@return void
function SkinnedMeshRenderer:BakeMesh(mesh, useScale) end
UnityEngine.SkinnedMeshRenderer = SkinnedMeshRenderer